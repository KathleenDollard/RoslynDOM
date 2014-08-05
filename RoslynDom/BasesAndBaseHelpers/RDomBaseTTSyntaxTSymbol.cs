using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class RDomBase<T, TSymbol> : RDomBase<T>, IRoslynDom<T, TSymbol>
          where TSymbol : ISymbol
          where T : class, IDom<T>
    {
        private SyntaxNode _originalRawSyntax;
        private SyntaxNode _rawSyntax;
        private TSymbol _symbol;
        private string _containingTypeName;

        protected RDomBase(SyntaxNode rawItem, IDom parent, SemanticModel model)
             : base()
        {
            _rawSyntax = rawItem;
            _originalRawSyntax = rawItem;
            Parent = parent;
            if (model != null)
            { _symbol = (TSymbol)model.GetDeclaredSymbol(rawItem); }
        }

        protected RDomBase(T oldIDom)
            : base(oldIDom)
        {
            var oldRDom = oldIDom as RDomBase<T, TSymbol>;
            _rawSyntax = oldRDom._rawSyntax;
            _originalRawSyntax = oldRDom._originalRawSyntax;
            _symbol = oldRDom._symbol;

            // TODO: SameIntent tests broke when I removed this, although it appears to be done in the base. 
            var thisAsHasName = this as IHasName;
            if (thisAsHasName != null)
            { thisAsHasName.Name = ((IHasName)oldRDom).Name; }

            Initialize();
        }

        public SyntaxNode TypedSyntax
        { get { return _rawSyntax; } }

        protected SyntaxNode OriginalTypedSyntax
        { get { return _originalRawSyntax; } }

        public override object RawItem
        { get { return _rawSyntax; } }

        public override object OriginalRawItem
        { get { return _originalRawSyntax; } }

        public override ISymbol Symbol
        { get { return TypedSymbol; } }

        public virtual TSymbol TypedSymbol
        { get { return _symbol; } }
       
        protected virtual AccessModifier GetAccessibility()
        {
            if (Symbol == null) { return AccessModifier.NotApplicable; }
            return (AccessModifier)Symbol.DeclaredAccessibility;
        }

        /// <summary>
        /// Fallback for getting requested values. 
        /// <br/>
        /// For special values (those that don't just return a property) override
        /// this method, return the approparite value, and olny call this base method when needed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
 

    }


}
