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
        private string _outerTypeName;

        internal RDomReferencedType(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
        {
            _raw = raw;
            _symbol = symbol;
            Initialize();
        }

        internal RDomReferencedType(TypeInfo typeInfo, ISymbol symbol)
        {
            _typeInfo = typeInfo;
            _symbol = symbol == null ? typeInfo.Type : symbol;
            Initialize();
        }

        internal RDomReferencedType(RDomReferencedType oldRDom)
             : base(oldRDom)
        {
            _symbol = oldRDom._symbol;
            _outerTypeName = oldRDom._outerTypeName;
            Name = oldRDom.Name;
            Namespace = oldRDom.Namespace;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Name = GetName();
            Namespace = GetNamespace();
            _outerTypeName = GetContainingTypeName();
        }


        private string GetName()
        {
            if (Symbol == null && (_typeInfo.Type != null)) { return _typeInfo.Type.ToString(); }
            var arraySymbol = Symbol as IArrayTypeSymbol;
            if (arraySymbol == null) { return Symbol.Name; }
            // CSharp specific
            return arraySymbol.ElementType.Name + "[]";
        }

        private string GetNamespace()
        { return GetNamespace(Symbol.ContainingNamespace); }

        private string GetNamespace(INamespaceSymbol nspaceSymbol)
        {
            if (nspaceSymbol == null) return "";
            var parentName = GetNamespace(nspaceSymbol.ContainingNamespace);
            if (!string.IsNullOrWhiteSpace(parentName))
            { parentName = parentName + "."; }
            return parentName + nspaceSymbol.Name;
        }

        private string GetContainingTypeName()
        { return GetContainingTypeName(Symbol.ContainingType); }

        private string GetContainingTypeName(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol == null) return "";
            var parentName = GetContainingTypeName(typeSymbol.ContainingType);
            return (string.IsNullOrWhiteSpace(parentName) ? "" : parentName + ".") +
                typeSymbol.Name;
        }

        public IReferencedType Copy()
        {
            return new RDomReferencedType(this);
        }

         internal override bool SameIntentInternal<TLocal>(TLocal other, bool includePublicAnnotations)
        {
            var otherItem = other as RDomReferencedType;
            if (otherItem == null) return false;
            if (!CheckSameIntent(otherItem, includePublicAnnotations)) return false;
            return true;
        }

        protected virtual bool CheckSameIntent(RDomReferencedType other, bool includePublicAnnotations)
        {
            var otherItem = other as RDomReferencedType;
            if (!base.CheckPublicAnnotations(otherItem, includePublicAnnotations)) return false;
            // The following is probably inadequate, but we need to find the edge cases
            if (this.QualifiedName != otherItem.QualifiedName) return false;
            return true;
        }
        public override object RawItem
        {
            // I want to understand how people are using this before exposing it
            get
            { throw new NotImplementedException(); }
        }

        public override ISymbol Symbol
        { get { return _symbol; } }

        public override string OuterName
        {
            get
            {
                return (string.IsNullOrWhiteSpace(_outerTypeName) ? "" : _outerTypeName + ".")
                    + Name;
            }
        }

        public string QualifiedName
        {
            get
            {
                return (string.IsNullOrWhiteSpace(Namespace) ? "" : Namespace + ".")
                    + (string.IsNullOrWhiteSpace(_outerTypeName) ? "" : _outerTypeName + ".")
                    + Name;
            }
        }

        public string Namespace { get; set; }

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
