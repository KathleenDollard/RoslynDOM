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
           : RDomMiscFactory<RDomReferencedType, TypeSyntax >
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
                if (syntaxNode.ChildTokens().Count() > 0)
                { CreateFromWorker.StoreWhitespaceForToken(newItem, syntaxNode.ChildTokens().First(), LanguagePart.Current, LanguageElement.Identifier); }

                newItem.Name = typeSyntax.ToString();
                //newItem.Name = GetName(newItem.Symbol);
                newItem.Namespace = GetNamespace(newItem.Symbol);
                return newItem;
            }
            throw new InvalidOperationException();
        }

        private string GetName(ISymbol symbol)
        {
            if (symbol == null) return null;
            var arraySymbol = symbol as IArrayTypeSymbol;
            if (arraySymbol == null) { return symbol.Name; }
            return arraySymbol.ElementType.Name + "[]";
        }


        private string GetNamespace(ISymbol symbol)
        { return symbol == null ? "" : GetNamespace(symbol.ContainingNamespace); }

        private string GetNamespace(INamespaceSymbol nspaceSymbol)
        {
            if (nspaceSymbol == null) return "";
            var parentName = GetNamespace(nspaceSymbol.ContainingNamespace);
            if (!string.IsNullOrWhiteSpace(parentName))
            { parentName = parentName + "."; }
            return parentName + nspaceSymbol.Name;
        }

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
            var node = SyntaxFactory.ParseTypeName(itemAsT.Name);

            node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, itemAsT.Whitespace2Set.First());
            node = BuildSyntaxHelpers.AttachWhitespaceToLast(node, itemAsT.Whitespace2Set.First());

            return node.PrepareForBuildSyntaxOutput(item);
        }


    }

}
