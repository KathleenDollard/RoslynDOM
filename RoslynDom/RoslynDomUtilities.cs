using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Reflection;

namespace RoslynDom
{
    public static class RoslynDomUtilities
    {

        public static IEnumerable<INamespace> GetAllChildNamespaces(
            IStemContainer stemContainer,
            bool includeSelf = false)
        {
            var ret = new List<INamespace>();
            if (includeSelf)
            {
                var nspace = stemContainer as INamespace;
                if (nspace != null) ret.Add(nspace);
            }
            foreach (var child in stemContainer.Namespaces)
            {
                ret.AddRange(GetAllChildNamespaces(child, true));
            }
            return ret;
        }

        public static IEnumerable<INamespace> GetNonEmptyNamespaces(
            IStemContainer stemContainer)
        {
            return GetAllChildNamespaces(stemContainer)
                .Where(x => x.StemMembers.Where(y => y.StemMemberKind != StemMemberKind.Namespace).Count() != 0);
        }

        public static IEnumerable<ITypeParameter> TypeParametersFrom(this INamedTypeSymbol rDomType)
        {
            return TypeParametersFrom(rDomType.TypeParameters);
        }

        public static IEnumerable<ITypeParameter> TypeParametersFrom(this IMethodSymbol rDomType)
        {
            return TypeParametersFrom(rDomType.TypeParameters);
        }

        private static IEnumerable<ITypeParameter> TypeParametersFrom(IEnumerable<ITypeParameterSymbol> typeParameters)
        {
            var retList = new List<ITypeParameter>();
            foreach (var param in typeParameters)
            {
                retList.Add(new RDomTypeParameter(param.DeclaringSyntaxReferences, param));
            }
            return retList;
        }

        public static IEnumerable<IReferencedType> ImpementedInterfacesFrom(this IHasImplementedInterfaces rDomType, bool all)
        {
            var symbol = ((IRoslynDom)rDomType).Symbol as INamedTypeSymbol;
            var retList = new List<IReferencedType>();
            IEnumerable<INamedTypeSymbol> interfaces;
            if (all) { interfaces = symbol.AllInterfaces; }
            else { interfaces = symbol.Interfaces; }
            foreach (var inter in interfaces)
            {
                retList.Add(new RDomReferencedType(inter.DeclaringSyntaxReferences, inter));

            }
            return retList;

        }

        public static void RemoveMemberFromParent(IDom parent, IDom member)
        {
            if (member.Parent != parent) { throw new InvalidOperationException(); }
            var memberAsRDom = member as RDomBase;
            if (memberAsRDom == null) { throw new InvalidOperationException(); }
            memberAsRDom.RemoveFromParent();
        }

        public static void PrepMemberForAdd(IDom parent, IDom member)
        {
            var memberAsRDom = member as RDomBase;
            if (memberAsRDom == null) throw new InvalidOperationException();
            if (member.Parent != null)
            {
                memberAsRDom.RemoveFromParent();
            }
            ReflectionUtilities.SetPropertyValue(memberAsRDom, "Parent", parent);
            //memberAsRDom.Parent = parent;
        }

        public static IEnumerable<T> CopyMembers<T>(IEnumerable<T> members)
        {
            var ret = new List<T>();
            if (members != null)
            {
                foreach (var member in members)
                {
                    ret.Add(Copy(member));
                }
            }
            return ret;
        }

        public static T Copy<T>(T oldItem)
        {
            var type = oldItem.GetType();
            var constructor = type.GetTypeInfo()
                .DeclaredConstructors
                .Where(x => x.GetParameters().Count() == 1
                && typeof(T).IsAssignableFrom(x.GetParameters().First().ParameterType))
                .FirstOrDefault();
            if (constructor == null) throw new InvalidOperationException("Missing constructor for clone");
            var newItem = constructor.Invoke(new object[] { oldItem });
            return (T)newItem;
        }

        public static string MakeOuterName(string outerTypeName, string name)
        {
            return (string.IsNullOrWhiteSpace(outerTypeName) ? "" : outerTypeName + ".")
                      + name;
        }

        public static string MakeQualifiedName(string nspace, string outerTypeName, string name)
        {
            return (string.IsNullOrWhiteSpace(nspace) ? "" : nspace + ".")
                      + MakeOuterName(outerTypeName, name);
        }

        public static string GetNamespace(IDom item)
        {
            var ret = "";
            if (item == null) { return ret; }
            var itemAsNamespace = item as INamespace;
            if (itemAsNamespace != null) { ret += itemAsNamespace.Name; }
            if (item.Parent == null || item.Parent is IRoot) { return ret; }
            var parentNamespace = GetNamespace(item.Parent);
            if (!string.IsNullOrEmpty(parentNamespace))
            { ret = parentNamespace + (string.IsNullOrEmpty(ret) ? "" : "." + ret); }
            return ret;
        }

        public static Type FindFirstSyntaxNodeType(Type type)
        {
            return FindFirstCastableType<SyntaxNode>(type);
            //if (type == typeof(object)) return null;
            //var syntaxType = type.GenericTypeArguments.Where(x => typeof(SyntaxNode).IsAssignableFrom(x)).FirstOrDefault();
            //if (syntaxType != null) { return syntaxType; }
            //return FindFirstSyntaxNode(syntaxType.BaseType);
        }

        public static Type FindFirstIDomType(Type type)
        {
            return FindFirstCastableType<IDom>(type);
            //if (type == typeof(object)) return null;
            //var syntaxType = type.GenericTypeArguments.Where(x => typeof(SyntaxNode).IsAssignableFrom(x)).FirstOrDefault();
            //if (syntaxType != null) { return syntaxType; }
            //return FindFirstSyntaxNode(syntaxType.BaseType);
        }

        public static Type FindFirstCastableType<T>(Type type)
        {
            if (type == typeof(object)) { return null; }
            var retType = type.GenericTypeArguments.Where(x => typeof(T).IsAssignableFrom(x)).FirstOrDefault();
            if (retType != null) { return retType; }
            return FindFirstCastableType<T>(type.BaseType);
        }

    }
}
