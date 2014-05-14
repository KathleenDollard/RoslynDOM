using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using RoslynDom.Implementations;
using RoslynDomUtilities;

namespace RoslynDom
{
    public static class RDomFactory
    {
        public static IRoot GetRootFromFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public static IRoot GetRootFromString(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            return GetRootFromSyntaxTree(tree);
        }

        public static IRoot GetRootFromSyntaxTree(SyntaxTree tree)
        {
            var root = MakeRoot(tree);
            return root;
        }


        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        /// <remarks>
        /// I'm not currently supporting attributes because I don't understand what attributes
        /// are on the compilation unit. Probably assembly attributes, but I want to get it first <br/>
        /// <br/>
        /// I currnelty think I will NOT support externs. They feel rare to me and something that the
        /// people who understand them can drop to the raw item to work with <br/>
        /// </remarks>
        private static RDomRoot MakeRoot(SyntaxTree tree)
        {
            // why are there attributes on a compilatoin unit?
            var kRoot = tree.GetRoot() as CompilationUnitSyntax;
            var members = ListUtilities.MakeList(kRoot, x => x.Members, x => MakeStemMember(x));
            var usings = ListUtilities.MakeList(kRoot, x => x.Usings, x => MakeUsingDirective(x));
            return new RDomRoot(kRoot, members, usings);
        }

        private static IUsing MakeUsingDirective(UsingDirectiveSyntax x)
        {
            return new RDomUsingDirective(x);
        }

        private static bool DoMember<T>(MemberDeclarationSyntax val, Func<T,
                IMember> doAction, out IMember retValue)
            where T : class
        {
            var item = val as T;
            retValue = item != null ? doAction(item) : null;
            return (retValue != null);
        }

        /// <summary>
        /// Creates namespace and class
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private static IStemMember MakeStemMember(MemberDeclarationSyntax rawMember)
        {
            var type = rawMember.GetType();
            IMember ret;
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMember<NamespaceDeclarationSyntax>(rawMember, MakeNamespace, out ret)) { }
            else if (DoMember<ClassDeclarationSyntax>(rawMember, MakeClass, out ret)) { }
            else if (DoMember<InterfaceDeclarationSyntax>(rawMember, MakeInterface, out ret)) { }
            else if (DoMember<StructDeclarationSyntax>(rawMember, MakeStructure, out ret)) { }
            else if (DoMember<EnumDeclarationSyntax>(rawMember, MakeEnum, out ret)) { }
            return ret as IStemMember;
        }

        /// <summary>
        /// Creates methods, properties and nested types
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private static ITypeMember MakeTypeMember(MemberDeclarationSyntax rawMember)
        {
            var type = rawMember.GetType();
            IMember ret;
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMember<MethodDeclarationSyntax>(rawMember, MakeMethod, out ret)) { }
            else if (DoMember<FieldDeclarationSyntax>(rawMember, MakeField, out ret)) { }
            else if (DoMember<PropertyDeclarationSyntax>(rawMember, MakeProperty, out ret)) { }
            else if (DoMember<ClassDeclarationSyntax>(rawMember, MakeClass, out ret)) { }
            else if (DoMember<StructDeclarationSyntax>(rawMember, MakeStructure, out ret)) { }
            else if (DoMember<EnumDeclarationSyntax>(rawMember, MakeEnum, out ret)) { }
            return ret as ITypeMember;
        }
        private static IMember MakeNamespace(NamespaceDeclarationSyntax rawNamespace)
        {
            var members = ListUtilities.MakeList(rawNamespace, x => x.Members, x => MakeStemMember(x));
            var usings = ListUtilities.MakeList(rawNamespace, x => x.Usings, x => MakeUsingDirective(x));
            return new RDomNamespace(rawNamespace, members, usings);
        }

        private static IMember MakeClass(ClassDeclarationSyntax rawClass)
        {
            var members = ListUtilities.MakeList(rawClass, x => x.Members, x => MakeTypeMember(x));
            return new RDomClass(rawClass, members);
        }

        private static IMember MakeStructure(StructDeclarationSyntax rawStruct)
        {
            var members = ListUtilities.MakeList(rawStruct, x => x.Members, x => MakeTypeMember(x));
            return new RDomStructure(rawStruct, members);
        }

        private static IMember MakeInterface(InterfaceDeclarationSyntax rawInterface)
        {
            var members = ListUtilities.MakeList(rawInterface, x => x.Members, x => MakeTypeMember(x));
            return new RDomInterface(rawInterface, members);
        }

        private static IMember MakeEnum(EnumDeclarationSyntax rawEnum)
        {
            return new RDomEnum(rawEnum);
        }
        private static IMember MakeMethod(MethodDeclarationSyntax rawMethod)
        {
            return new RDomMethod(rawMethod);
        }

        private static IMember MakeProperty(PropertyDeclarationSyntax rawProperty)
        {
            return new RDomProperty(rawProperty);
        }

        private static IMember MakeField(FieldDeclarationSyntax rawField)
        {
            return new RDomField(rawField);
        }
        #endregion
    }
}
