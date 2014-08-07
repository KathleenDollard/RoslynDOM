using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomReferencedTypeMiscFactory
           : RDomMiscFactory<RDomReferencedType, SyntaxNode>
    {
        public RDomReferencedTypeMiscFactory(RDomCorporation corporation)
            :base (corporation)
        { }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var typeParameterSyntax = syntaxNode as TypeParameterSyntax;
            if (typeParameterSyntax != null) throw new NotImplementedException("Should have called TypeParameterFactory");
            var typeSyntax = syntaxNode as TypeSyntax;
            if (typeSyntax != null)
            {
                var newItem = new RDomReferencedType(syntaxNode, parent, model);
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
        { return symbol == null ? "" :  GetNamespace(symbol.ContainingNamespace); }

        private string GetNamespace(INamespaceSymbol nspaceSymbol)
        {
            if (nspaceSymbol == null) return "";
            var parentName = GetNamespace(nspaceSymbol.ContainingNamespace);
            if (!string.IsNullOrWhiteSpace(parentName))
            { parentName = parentName + "."; }
            return parentName + nspaceSymbol.Name;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IReferencedType;
            var node =  SyntaxFactory.ParseTypeName(itemAsT.Name);
            return node.PrepareForBuildSyntaxOutput(item);
        }

   
    }

}
