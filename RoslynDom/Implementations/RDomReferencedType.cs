using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReferencedType : RDomBase, IReferencedType
    {
        private ImmutableArray<SyntaxReference> _raw;
        private ISymbol _symbol;


        internal RDomReferencedType(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
        {
            _raw = raw;
            _symbol = symbol;
        }

        public override object RawSyntax
        {
            get
            { return _raw; }
        }

        public override ISymbol Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public override string Name
        {
            get
            {
                return Symbol.Name;
            }
        }

        public override string OuterName
        {
            get
            {
                // namespace overrides this
                var typeName = GetContainingTypeName(Symbol.ContainingType);
                return (string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".") +
                       Name;
            }
        }


        public override string QualifiedName
        {
            get
            {
                var namespaceName = GetContainingNamespaceName(Symbol.ContainingNamespace);
                var typeName = GetContainingTypeName(Symbol.ContainingType);
                namespaceName = string.IsNullOrWhiteSpace(namespaceName) ? "" : namespaceName + ".";
                typeName = string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".";
                return namespaceName + typeName + Name;
            }
        }

        public override string Namespace
        {
            get
            {
                return GetContainingNamespaceName(Symbol.ContainingNamespace);
                //var namespaceName = GetContainingNamespaceName(Symbol.ContainingNamespace);
                //var typeName = GetContainingTypeName(Symbol.ContainingType);
                //return (string.IsNullOrWhiteSpace(namespaceName) ? "" : namespaceName + ".") +
                //       (string.IsNullOrWhiteSpace(typeName) ? "" : typeName + ".") +
                //       Name;
            }
        }

        internal string GetContainingNamespaceName(INamespaceSymbol nspaceSymbol)
        // TODO: Change to assembly protected when it is available
        {
            if (nspaceSymbol == null) return "";
            var parentName = GetContainingNamespaceName(nspaceSymbol.ContainingNamespace);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                nspaceSymbol.Name;
        }

        private string GetContainingTypeName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null) return "";
            var parentName = GetContainingTypeName(typeSymbol.ContainingType);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                typeSymbol.Name;
        }

    }
}
