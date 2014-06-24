using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    internal static class RoslynDomUtilities
    {
        internal static string GetContainingNamespaceName(INamespaceSymbol nspaceSymbol)
        {
            if (nspaceSymbol == null) return "";
            var parentName = GetContainingNamespaceName(nspaceSymbol.ContainingNamespace);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                nspaceSymbol.Name;
        }

        internal static IEnumerable<INamespace> GetAllChildNamespaces(
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

        internal static IEnumerable < INamespace > GetNonEmptyNamespaces(
            IStemContainer stemContainer)
        {
            return GetAllChildNamespaces(stemContainer)
                .Where(x => x.Members.Where(y=>y.StemMemberType!=StemMemberType.Namespace).Count() != 0);
        }

        internal static IEnumerable<ITypeParameter> TypeParametersFrom(this INamedTypeSymbol rDomType)
        {
            return TypeParametersFrom(rDomType.TypeParameters);
        }

        internal static IEnumerable<ITypeParameter> TypeParametersFrom(this IMethodSymbol rDomType)
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
            //throw new NotImplementedException();
            return retList;
        }

        internal static IEnumerable<IReferencedType> ImpementedInterfacesFrom(this IHasImplementedInterfaces rDomType, bool all)
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
    }
}
