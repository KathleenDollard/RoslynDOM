using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public abstract class RDomDetail<T> : RDomBase<T>, IDetail<T>
       where T : class, IDetail<T>
   {
      private SyntaxTrivia _syntaxTrivia;
      private IDom _parent;

      internal RDomDetail(IDom parent, StemMemberKind stemMemberKind, MemberKind memberKind, SyntaxTrivia trivia)
      {
         _syntaxTrivia = trivia;
         _stemMemberKind = stemMemberKind;
         _memberKind = memberKind;
         _parent = parent;
      }

      internal RDomDetail(T oldRDom)
         : base(oldRDom)
      {
         _stemMemberKind = oldRDom.StemMemberKind;
         _memberKind = oldRDom.MemberKind;
      }

      private StemMemberKind _stemMemberKind;

      public StemMemberKind StemMemberKind
      {
         get { return _stemMemberKind; }
      }

      private MemberKind _memberKind;

      public MemberKind MemberKind
      {
         get { return _memberKind; }
      }

      public override object OriginalRawItem
      { get { return null; } }

      public override object RawItem
      { get { return null; } }

      public override object RequestValue(string propertyName)
      { return null; }

      public override ISymbol Symbol
      { get { return null; } }

      public object Trivia
      { get { return _syntaxTrivia; } }

      public SyntaxTrivia TypedTrivia
      { get { return _syntaxTrivia; } }
   }
}