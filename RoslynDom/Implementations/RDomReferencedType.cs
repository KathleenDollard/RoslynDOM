using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This does not inherit from <see cref="RDomBase{T, TSymbol}"/> because
    /// it does not have a property SyntaxNode to reference. 
    /// </remarks>
    public class RDomReferencedType : RDomBase<IReferencedType>, IReferencedType
    {
        // I'm still evolving how types are handled.
        private ImmutableArray<SyntaxReference> _raw;
        private TypeInfo _typeInfo;
        private ISymbol _symbol;
        private string _outerTypeName;
     //   private ISameIntent<IReferencedType> sameIntent = SameIntent_Factory.SameIntent<IReferencedType>();

        public RDomReferencedType(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
        {
            _raw = raw;
            _symbol = symbol;
            Initialize();
        }

        public RDomReferencedType(TypeInfo typeInfo, ISymbol symbol)
        {
            _typeInfo = typeInfo;
            _symbol = symbol == null ? typeInfo.Type : symbol;
            Initialize();
        }

        public RDomReferencedType(RDomReferencedType oldRDom)
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

        public virtual bool Matches(IReferencedType other)
        { return this.Name == other.Name; }

        //public IReferencedType Copy()
        //{
        //    return new RDomReferencedType(this);
        //}

        //protected override bool SameIntentInternal<TLocal>(TLocal other, bool skipPublicAnnotations)
        //{
        //    var thisAsT = this as IPublicAnnotation;
        //    var otherAsT = other as IPublicAnnotation;
        //    if (!CheckSameIntent(other as IReferencedType, skipPublicAnnotations)) { return false; }
        //    return (StandardSameIntent.CheckSameIntent(thisAsT, otherAsT, skipPublicAnnotations))  ;
        //}

        //protected virtual bool CheckSameIntent(IReferencedType other, bool skipPublicAnnotations)
        //{
        //    return true;
        //}

        public override object RawItem
        {
            // I want to understand how people are using this before exposing it
            get
            { throw new NotImplementedException(); }
        }

        public override object OriginalRawItem
        {
            // I want to understand how people are using this before exposing it
            get
            { throw new NotImplementedException(); }
        }

        public override ISymbol Symbol
        { get { return _symbol; } }

        public string Name { get; set; }

        public string OuterName
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

        private string GetName()
        {
            if (Symbol == null && (_typeInfo.Type != null)) { return _typeInfo.Type.ToString(); }
            var arraySymbol = Symbol as IArrayTypeSymbol;
            if (arraySymbol == null) { return Symbol.Name; }
            // CSharp specific
            return arraySymbol.ElementType.Name + "[]";
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
    }
}
