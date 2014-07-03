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

        public static IRoot GetRootFromDocument(Document document)
        {
            if (document == null) { throw new InvalidOperationException(); }
            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
            return GetRootFromSyntaxTree(tree);
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
            var publicAnnotations = GetPublicAnnotations(kRoot).ToArray();
            return new RDomRoot(kRoot, members, usings, publicAnnotations);
        }


        private static IUsing MakeUsingDirective(UsingDirectiveSyntax x)
        {
            var publicAnnotations = GetPublicAnnotations(x).ToArray();
            return new RDomUsingDirective(x, publicAnnotations);
        }

        private static bool DoMember<T>(MemberDeclarationSyntax val,
                Func<T, IMember> doAction, out IMember retValue)
            where T : class
        {
            var item = val as T;
            retValue = item != null ? doAction(item) : null;
            return (retValue != null);
        }

        private static bool DoMembers<T>(MemberDeclarationSyntax val,
                Action<T, List<IMember>> doAction, List<IMember> list)
            where T : class
        {
            var item = val as T;
            if (item == null) return false;
            var startCount = list.Count();
            doAction(item, list);
            return (list.Count() > startCount);
        }

        /// <summary>
        /// Creates namespace and class
        /// </summary>
        /// <param name="rawMember"></param>
        /// <returns></returns>
        private static IStemMember MakeStemMember(MemberDeclarationSyntax rawMember)
        {
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
        private static IEnumerable<ITypeMember> MakeTypeMembers(MemberDeclarationSyntax rawMember)
        {
            IMember item;
            var retList = new List<IMember>();
            // The action happens in DoMember. I felt it read better with else than negation and it made copying new lines easier
            if (DoMember<MethodDeclarationSyntax>(rawMember, MakeMethod, out item)) { }
            else if (DoMember<PropertyDeclarationSyntax>(rawMember, MakeProperty, out item)) { }
            else if (DoMember<ClassDeclarationSyntax>(rawMember, MakeClass, out item)) { }
            else if (DoMember<InterfaceDeclarationSyntax>(rawMember, MakeInterface, out item)) { }
            else if (DoMember<StructDeclarationSyntax>(rawMember, MakeStructure, out item)) { }
            else if (DoMember<EnumDeclarationSyntax>(rawMember, MakeEnum, out item)) { }
            else if (DoMembers<FieldDeclarationSyntax>(rawMember, MakeFields, retList)) { }
            else if (DoMember<SyntaxNode>(rawMember, MakeInvalidMember, out item)) { }
            else throw new NotImplementedException();
            if (item != null) retList.Add(item);
            return retList.OfType<ITypeMember>();
        }

        private static IMember MakeNamespace(NamespaceDeclarationSyntax rawNamespace)
        {
            var members = ListUtilities.MakeList(rawNamespace, x => x.Members, x => MakeStemMember(x));
            var usings = ListUtilities.MakeList(rawNamespace, x => x.Usings, x => MakeUsingDirective(x));
            var publicAnnotations = GetPublicAnnotations(rawNamespace).ToArray();
            return new RDomNamespace(rawNamespace, members, usings, publicAnnotations);
        }

        private static IMember MakeClass(ClassDeclarationSyntax rawClass)
        {
            var members = ListUtilities.MakeList(rawClass, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = GetPublicAnnotations(rawClass).ToArray();
            return new RDomClass(rawClass, members, publicAnnotations);
        }

        private static IMember MakeStructure(StructDeclarationSyntax rawStruct)
        {
            var members = ListUtilities.MakeList(rawStruct, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = GetPublicAnnotations(rawStruct).ToArray();
            return new RDomStructure(rawStruct, members, publicAnnotations);
        }

        private static IMember MakeInterface(InterfaceDeclarationSyntax rawInterface)
        {
            var members = ListUtilities.MakeList(rawInterface, x => x.Members, x => MakeTypeMembers(x));
            var publicAnnotations = GetPublicAnnotations(rawInterface).ToArray();
            return new RDomInterface(rawInterface, members, publicAnnotations);
        }

        private static IMember MakeEnum(EnumDeclarationSyntax rawEnum)
        {
            var publicAnnotations = GetPublicAnnotations(rawEnum).ToArray();
            return new RDomEnum(rawEnum, publicAnnotations);
        }
        private static IMember MakeMethod(MethodDeclarationSyntax rawMethod)
        {
            var parms = ListUtilities.MakeList(rawMethod, x => x.ParameterList.Parameters, x => MakeParameter(x));
            var publicAnnotations = GetPublicAnnotations(rawMethod).ToArray();
            return new RDomMethod(rawMethod, parms, publicAnnotations);
        }

        private static IParameter MakeParameter(ParameterSyntax rawParm)
        {
            var publicAnnotations = GetPublicAnnotations(rawParm).ToArray();
            return new RDomParameter(rawParm, publicAnnotations);
        }

        private static IMember MakeProperty(PropertyDeclarationSyntax rawProperty)
        {
            // VB will have property parameters
            var parms = new List<IParameter>();
            var publicAnnotations = GetPublicAnnotations(rawProperty).ToArray();
            return new RDomProperty(rawProperty, parms, publicAnnotations);
        }

        private static IMember MakeInvalidMember(SyntaxNode rawItem)
        {
            var publicAnnotations = GetPublicAnnotations(rawItem).ToArray();
            return new RDomInvalidTypeMember(rawItem, publicAnnotations);
        }

        private static void MakeFields(FieldDeclarationSyntax rawField, List<IMember> list)
        {
            var declarators = rawField.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var publicAnnotations = GetPublicAnnotations(rawField).ToArray();
                list.Add(new RDomField(rawField, decl, publicAnnotations));
            }
        }

        #endregion

        #region Private methods to support public annotations
        private static IEnumerable<PublicAnnotation> GetPublicAnnotations(CompilationUnitSyntax kRoot)
        {
            var ret = new List<PublicAnnotation>();
            var nodes = kRoot.ChildNodes();
            foreach (var node in nodes)
            {
                ret.AddRange(GetPublicAnnotationFromFirstToken(node, true));
            }
            return ret;
        }

        private static IEnumerable<PublicAnnotation> GetPublicAnnotations(SyntaxNode node)
        {
            return GetPublicAnnotationFromFirstToken(node, false);
        }
        private static IEnumerable<PublicAnnotation> GetPublicAnnotationFromFirstToken(
                   SyntaxNode node, bool isRoot)
        {
            var ret = new List<PublicAnnotation>();
            var firstToken = node.GetFirstToken();
            if (firstToken != default(SyntaxToken))
            {
                ret.AddRange(GetPublicAnnotationFromToken(firstToken, isRoot));
            }
            return ret;
        }

        private static IEnumerable<PublicAnnotation> GetPublicAnnotationFromToken(
               SyntaxToken token, bool isRoot)
        {
            var ret = new List<PublicAnnotation>();
            var trivias = token.LeadingTrivia
                              .Where(x => x.CSharpKind() == SyntaxKind.SingleLineCommentTrivia);
            foreach (var trivia in trivias)
            {
                var str = GetPublicAnnotationAsString(trivia);
                var strRoot = GetSpecialRootAnnotation(str);
                if (isRoot)
                { str = strRoot; }
                else
                { str = string.IsNullOrWhiteSpace(strRoot) ? str : ""; }
                if (!string.IsNullOrWhiteSpace(str))
                {
                    var attrib = GetAnnotationStringAsAttribute(str);
                    var newPublicAnnotation = new PublicAnnotation(attrib.Name.ToString());
                    var args = attrib.ArgumentList.Arguments;
                    foreach (var arg in args)
                    {
                        // reuse parsing
                        var tempRDomAttributeValue = new RDomAttributeValue(arg, attrib, null);
                        newPublicAnnotation.AddItem(tempRDomAttributeValue.Name, tempRDomAttributeValue.Value);
                    }
                    ret.Add(newPublicAnnotation);
                }
            }
            return ret;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(System.String,System.String,Microsoft.CodeAnalysis.CSharp.CSharpParseOptions,System.Threading.CancellationToken)")]
        private static AttributeSyntax GetAnnotationStringAsAttribute(string str)
        {
            // Trick Roslyn into thinking it's an attribute
            str = "[" + str + "] public class {}";
            var tree = CSharpSyntaxTree.ParseText(str);
            var attrib = tree.GetRoot().DescendantNodes()
                        .Where(x => x.CSharpKind() == SyntaxKind.Attribute)
                        .FirstOrDefault();
            return attrib as AttributeSyntax;
        }

        private static string GetPublicAnnotationAsString(SyntaxTrivia trivia)
        {
            var str = trivia.ToString().Trim();
            if (!str.StartsWith("//", StringComparison.Ordinal)) throw new InvalidOperationException("Unexpected comment format");
            str = str.SubstringAfter("//").SubstringAfter("[[").SubstringBefore("]]").Trim();
            return str;
        }

        private static string GetSpecialRootAnnotation(string str)
        {
            str = str.Trim();

            if (str.StartsWith("file:", StringComparison.Ordinal))
            { return str.SubstringAfter("file:"); }
            if (str.StartsWith("root:", StringComparison.Ordinal))
            { return str.SubstringAfter("root:"); }
            return null;
        }
        #endregion
    }
}
