using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using RoslynDom;
using RoslynDomUtilities;

namespace RoslynDom
{
    public static class RDomFactory
    {
        public static IRoot GetRootFromFile(string fileName)
        {
            var code = File.ReadAllText(fileName);
            return GetRootFromString(code);
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


        private static bool DoMembers<T>(MemberDeclarationSyntax val,
                    Action<T, List<IMember>> doAction, List<IMember> list)
            where T : class
        {
            var item = val as T;
            if (item == null) return false;
            doAction(item, list);
            return (list.Count() > 0);
        }

        /// <summary>
        /// Creates namespace and class
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private static IEnumerable<IStemMember> MakeStemMember(MemberDeclarationSyntax rawMember)
        {
            var type = rawMember.GetType();
            var retList = new List<IMember>(); // This list is modified in DoMembers
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMembers<NamespaceDeclarationSyntax>(rawMember, MakeNamespace, retList)) { }
            else if (DoMembers<ClassDeclarationSyntax>(rawMember, MakeClass, retList)) { }
            else if (DoMembers<InterfaceDeclarationSyntax>(rawMember, MakeInterface, retList)) { }
            else if (DoMembers<StructDeclarationSyntax>(rawMember, MakeStructure, retList)) { }
            else if (DoMembers<EnumDeclarationSyntax>(rawMember, MakeEnum, retList)) { }
            return retList.OfType<IStemMember>();
        }

        /// <summary>
        /// Creates methods, properties and nested types
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private static IEnumerable<ITypeMember> MakeTypeMembers(MemberDeclarationSyntax rawMember)
        {
            var type = rawMember.GetType();
            var retList = new List<IMember>(); // This list is modified in DoMembers
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMembers<MethodDeclarationSyntax>(rawMember, MakeMethod, retList)) { }
            else if (DoMembers<FieldDeclarationSyntax>(rawMember, MakeField, retList)) { }
            else if (DoMembers<PropertyDeclarationSyntax>(rawMember, MakeProperty, retList)) { }
            else if (DoMembers<ClassDeclarationSyntax>(rawMember, MakeClass, retList)) { }
            else if (DoMembers<InterfaceDeclarationSyntax>(rawMember, MakeInterface, retList)) { }
            else if (DoMembers<StructDeclarationSyntax>(rawMember, MakeStructure, retList)) { }
            else if (DoMembers<EnumDeclarationSyntax>(rawMember, MakeEnum, retList)) { }
            return retList.OfType<ITypeMember>();
        }

        private static IUsing MakeUsingDirective(UsingDirectiveSyntax x)
        {
            return new RDomUsingDirective(x);
        }

        private static void MakeNamespace(NamespaceDeclarationSyntax rawNamespace, List<IMember> list)
        {
            var members = ListUtilities.MakeList(rawNamespace, x => x.Members, x => MakeStemMember(x));
            var usings = ListUtilities.MakeList(rawNamespace, x => x.Usings, x => MakeUsingDirective(x));
            list.Add(new RDomNamespace(rawNamespace, members, usings));
        }

        private static void MakeClass(ClassDeclarationSyntax rawClass, List<IMember> list)
        {
            var members = ListUtilities.MakeList(rawClass, x => x.Members, x => MakeTypeMembers(x));
            list.Add(new RDomClass(rawClass, members));
        }

        private static void MakeStructure(StructDeclarationSyntax rawStruct, List<IMember> list)
        {
            var members = ListUtilities.MakeList(rawStruct, x => x.Members, x => MakeTypeMembers(x));
            list.Add(new RDomStructure(rawStruct, members));
        }

        private static void MakeInterface(InterfaceDeclarationSyntax rawInterface, List<IMember> list)
        {
            var members = ListUtilities.MakeList(rawInterface, x => x.Members, x => MakeTypeMembers(x));
            list.Add(new RDomInterface(rawInterface, members));
        }

        private static void MakeEnum(EnumDeclarationSyntax rawEnum, List<IMember> list)
        {
            list.Add(new RDomEnum(rawEnum));
        }
        private static void MakeMethod(MethodDeclarationSyntax rawMethod, List<IMember> list)
        {
            list.Add(new RDomMethod(rawMethod));
        }

        private static void MakeProperty(PropertyDeclarationSyntax rawProperty, List<IMember> list)
        {
            list.Add(new RDomProperty(rawProperty));
        }

        private static void MakeField(FieldDeclarationSyntax rawField, List<IMember> list)
        {
            list.Add(new RDomField(rawField));
        }
        #endregion
    }
}
