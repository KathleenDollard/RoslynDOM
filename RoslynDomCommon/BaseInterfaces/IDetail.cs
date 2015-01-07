namespace RoslynDom.Common
{
   /// <summary>
   ///
   /// </summary>
   public interface IDetail : IStemMemberAndDetail, ITypeMemberAndDetail, IStatementAndDetail, IMisc
   {
      object Trivia { get; }
   }

   /// <summary>
   ///
   /// </summary>
   public interface IDetail<T> : IDetail, IStemMemberAndDetail, ITypeMemberAndDetail, IStatementAndDetail, IDom<T>
         where T : IDetail<T>
   {
   }
}