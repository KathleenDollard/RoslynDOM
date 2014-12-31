using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
   public abstract class RDomBase<T, TSymbol> : RDomBase<T>, IRoslynDom<T, TSymbol>
      where T : class, IDom<T>
      where TSymbol : ISymbol
   {
      private TSymbol _symbol;

      protected RDomBase()
         : this(null, null, null)
      { NeedsFormatting  = true; }

      protected RDomBase(IDom parent)
         : this(null, parent, null)
      { NeedsFormatting = true; }

      protected RDomBase(SyntaxNode rawSyntax, IDom parent, SemanticModel model)
         : base(rawSyntax)
      {
         Parent = parent;
         if (model != null)
         {
            _symbol = (TSymbol)model.GetDeclaredSymbol(rawSyntax);
            if (_symbol == null)
            { _symbol = (TSymbol)model.GetSymbolInfo(rawSyntax).Symbol; }
            if (_symbol == null)
            { _symbol = (TSymbol)model.GetTypeInfo(rawSyntax).Type; }
         }
      }

      protected RDomBase(T oldIDom)
         : base(oldIDom)
      {
         var oldRDom = oldIDom as RDomBase<T, TSymbol>;
         _symbol = oldRDom._symbol;

         // TODO: SameIntent tests broke when I removed this, although it appears to be done in the base. 
         var thisAsHasName = this as IHasName;
         if (thisAsHasName != null)
         { thisAsHasName.Name = ((IHasName)oldRDom).Name; }
      }

      public SyntaxNode TypedSyntax
      { get { return RawItem as SyntaxNode ; } }

      public SyntaxNode OriginalTypedSyntax
      { get { return OriginalRawItem as SyntaxNode ; } }

       public override ISymbol Symbol
      { get { return TypedSymbol; } }

      public virtual TSymbol TypedSymbol
      {
         get { return _symbol; }
      }
   }
}