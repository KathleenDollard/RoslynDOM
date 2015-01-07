using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
   public abstract class RDomBase<T, TSymbol> : RDomBase<T>, IRoslynDom<T, TSymbol>
      where T : class, IDom<T>
      where TSymbol : ISymbol
   {
      private SyntaxNode _originalRawSyntax;
      private SyntaxNode _rawSyntax;
      private TSymbol _symbol;

      protected RDomBase()
         : this(null, null, null)
      { NeedsFormatting = true; }

      protected RDomBase(IDom parent)
         : this(null, parent, null)
      { NeedsFormatting = true; }

      protected RDomBase(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base()
      {
         _rawSyntax = rawItem;
         _originalRawSyntax = rawItem;
         Parent = parent;
         if (model != null)
         {
            _symbol = (TSymbol)model.GetDeclaredSymbol(rawItem);
            if (_symbol == null)
            { _symbol = (TSymbol)model.GetSymbolInfo(rawItem).Symbol; }
            if (_symbol == null)
            { _symbol = (TSymbol)model.GetTypeInfo(rawItem).Type; }
         }
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
      }

      public SyntaxNode TypedSyntax
      { get { return _rawSyntax; } }

      public SyntaxNode OriginalTypedSyntax
      { get { return _originalRawSyntax; } }

      public override object RawItem
      { get { return _rawSyntax; } }

      public override object OriginalRawItem
      { get { return _originalRawSyntax; } }

      public override ISymbol Symbol
      { get { return TypedSymbol; } }

      public virtual TSymbol TypedSymbol
      {
         get { return _symbol; }
         set { _symbol = value; }
      }
   }
}