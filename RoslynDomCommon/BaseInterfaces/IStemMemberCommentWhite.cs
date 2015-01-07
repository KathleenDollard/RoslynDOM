namespace RoslynDom.Common
{
   public interface IStemMemberAndDetail : IDom
   {
      StemMemberKind StemMemberKind { get; }
   }
}