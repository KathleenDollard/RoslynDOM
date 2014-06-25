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
        private TypeInfo _typeInfo;
        private ISymbol _symbol;


        internal RDomReferencedType(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
        {
            _raw = raw;
            _symbol = symbol;
        }


        internal RDomReferencedType(TypeInfo typeInfo, ISymbol symbol)
        {
            _typeInfo = typeInfo;
            _symbol = symbol;
        }

        public override object RawItem
        {
            // I want to understand how people are using this before exposing it
            get
            { throw new NotImplementedException(); }
        }

        public override ISymbol Symbol
        { get { return _symbol; } }

        public override string Name
        {
            get
            {
                if (Symbol == null && (_typeInfo.Type != null )) { return _typeInfo.Type.ToString(); }
                var arraySymbol = Symbol as IArrayTypeSymbol;
                if (arraySymbol == null) { return Symbol.Name; }
                // CSharp specific
                return arraySymbol.ElementType.Name + "[]";
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
                //return Symbol.ToDisplayString();
                //return Symbol.ToString();
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
            if (!string.IsNullOrWhiteSpace(parentName))
            { parentName = parentName + "."; }
            return parentName + nspaceSymbol.Name;
        }

        private string GetContainingTypeName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null ) return "";
            var parentName = GetContainingTypeName(typeSymbol.ContainingType);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                typeSymbol.Name;
        }

        public override object RequestValue(string name)
        {  // This is temporary so I know how this is used. Probably can just be removed and fallback to base
            throw new NotImplementedException();
        }

        internal override ISymbol GetSymbol(SyntaxNode node)
        {
            throw new NotImplementedException();
        }
    }
}
