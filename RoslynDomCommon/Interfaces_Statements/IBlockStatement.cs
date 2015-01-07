namespace RoslynDom.Common
{
   public interface IBlockStatement : IStatement, IDom<IBlockStatement>
   {
      RDomCollection<IStatementAndDetail> Statements { get; }
   }
}