using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomReferencedTypeMiscFactory
           : RDomMiscFactory<RDomReferencedType, TypeSyntax>
    {
        private static WhitespaceKindLookup whitespaceLookup;

        public RDomReferencedTypeMiscFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var typeParameterSyntax = syntaxNode as TypeParameterSyntax;
            if (typeParameterSyntax != null) throw new NotImplementedException("Should have called TypeParameterFactory");
            var typeSyntax = syntaxNode as TypeSyntax;
            if (typeSyntax != null)
            {
                var newItem = new RDomReferencedType(syntaxNode, parent, model);

                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
                CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntaxNode, LanguagePart.Current, LanguageElement.Identifier);

                var arrayTypeSyntax = syntaxNode as ArrayTypeSyntax;
                if (arrayTypeSyntax != null)
                {
                    newItem.IsArray = true;
                    var arraySymbol = newItem.Symbol as IArrayTypeSymbol;
                    InitalizeNameAndNamespace(newItem, arraySymbol.ElementType, arrayTypeSyntax.ElementType);
                }
                else
                {
                    newItem.IsArray = false;
                    InitalizeNameAndNamespace(newItem, newItem.Symbol, typeSyntax);
                }
                return newItem;
            }
            throw new InvalidOperationException();
        }

        private void InitalizeNameAndNamespace(RDomReferencedType newItem,ISymbol symbol, TypeSyntax typeSyntax)
        {
            var predefinedSyntax = typeSyntax as PredefinedTypeSyntax;
            if (predefinedSyntax != null) { InitializeFromPredefined(newItem, symbol, predefinedSyntax); }
            else if (symbol != null) { InitializeFromSymbol(newItem, symbol, typeSyntax); }
            else { InitializeFromCode(newItem, typeSyntax); }
        }

        private void InitializeFromCode(RDomReferencedType newItem, TypeSyntax typeSyntax)
        {
            var text = typeSyntax.ToString();
            if (text.Contains("."))
            {
                // Assume no nested types as there is no way to distinguish
                newItem.Name = typeSyntax.ToString().SubstringAfterLast(".");
                newItem.Namespace = typeSyntax.ToString().SubstringBeforeLast(".");
            }
            else
            {
                newItem.Name = text;
                newItem.Namespace = "";
            }
        }

        private void InitializeFromSymbol(RDomReferencedType newItem, ISymbol symbol, TypeSyntax typeSyntax)
        {
            newItem.Name = symbol.Name;

            if (symbol.ContainingType == null)
            { newItem.ContainingTypeName = ""; }
            else
            { newItem.ContainingTypeName = symbol.ContainingType.Name; }

            if (symbol.ContainingNamespace == null
                || symbol.ContainingNamespace.ToString() == "<global namespace>")
            { newItem.Namespace = ""; }
            else
            { newItem.Namespace = symbol.ContainingNamespace.ToString(); }
        }

        private void InitializeFromPredefined(RDomReferencedType newItem, ISymbol symbol, PredefinedTypeSyntax predefinedSyntax)
        {
            newItem.DisplayAlias = true;
            newItem.ContainingTypeName = "";  // No predefined have containing types

            newItem.Name = symbol.Name;
            if (symbol == null) throw new NotImplementedException();
            //var arrayTypeSymbol = symbol as IArrayTypeSymbol;
            //if (arrayTypeSymbol != null)
            //{
            //    symbol = arrayTypeSymbol.BaseType;
            //}
            newItem.Namespace = symbol.ContainingNamespace.ToString();
        }

        //private string GetName(TypeSyntax typeSyntax, ISymbol symbol)
        //{
        //    if (symbol == null) return null;
        //    var arraySymbol = symbol as IArrayTypeSymbol;
        //    if (arraySymbol == null) { return symbol.Name; }
        //    return arraySymbol.ElementType.Name + "[]";
        //}


        //private string GetNamespace(ISymbol symbol)
        //{ return symbol == null ? "" : GetNamespace(symbol.ContainingNamespace); }

        //private string GetNamespace(INamespaceSymbol nspaceSymbol)
        //{
        //    if (nspaceSymbol == null) return "";
        //    var parentName = GetNamespace(nspaceSymbol.ContainingNamespace);
        //    if (!string.IsNullOrWhiteSpace(parentName))
        //    { parentName = parentName + "."; }
        //    return parentName + nspaceSymbol.Name;
        //}

        //private void StoreWhitespaceForToken(IDom newItem, SyntaxToken token, LanguageElement languageElement)
        //{
        //    var newWS = new Whitespace2(LanguagePart.Current, languageElement);
        //    newWS.LeadingWhitespace = token.LeadingTrivia
        //                                .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia)
        //                                .Select(x => x.ToString())
        //                                .JoinString();
        //    newWS.TrailingWhitespace = token.TrailingTrivia
        //                                .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia
        //                                        || x.CSharpKind() == SyntaxKind.EndOfLineTrivia)
        //                                .Select(x => x.ToString())
        //                                .JoinString();
        //    // TODO: Add EOL comments here
        //    newItem.Whitespace2Set[languageElement] = newWS;
        //}



        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IReferencedType;
            var node = TypeSyntaxFromType(itemAsT);

            node = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(node, itemAsT.Whitespace2Set.FirstOrDefault());

            return node.PrepareForBuildSyntaxOutput(item);
        }

        // Not sure if this belongs here or in BuildSyntaxHelpers
        public static TypeSyntax TypeSyntaxFromType(IReferencedType type)
        {
            string typeName = type.QualifiedName;
            if (type.DisplayAlias)
            {
                switch (type.QualifiedName)
                {
                    case "Void":
                    case "System.Void":  { typeName = "void"; break; }
                    case "System.Object": { typeName = "object"; break; }
                    case "System.String": { typeName = "string"; break; }
                    case "System.Boolean": { typeName = "bool"; break; }
                    case "System.Decimal": { typeName = "decimal"; break; }
                    case "System.SByte": { typeName = "sbyte"; break; }
                    case "System.Byte": { typeName = "byte"; break; }
                    case "System.Int16": { typeName = "short"; break; }
                    case "System.UInt16": { typeName = "ushort"; break; }
                    case "System.Int32": { typeName = "int"; break; }
                    case "System.UInt32": { typeName = "uint"; break; }
                    case "System.Int64": { typeName = "long"; break; }
                    case "System.UInt64": { typeName = "ulong"; break; }
                    case "System.Char": { typeName = "char"; break; }
                    case "System.Single": { typeName = "float"; break; }
                    case "System.Double": { typeName = "double"; break; }
                    default:
                    break;
                }
            }
            if (type.IsArray )
            { typeName += "[]"; }
            return SyntaxFactory.ParseTypeName(typeName);

        }

    }

}
