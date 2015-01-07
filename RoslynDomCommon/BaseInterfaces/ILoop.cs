namespace RoslynDom.Common
{
   public interface ILoop : IStatement, IHasCondition, IStatementBlock
   {
      bool TestAtEnd { get; set; }
   }

   public interface ILoop<T> : IDom<T>, ILoop
       where T : IDom<T>
   {
   }
}