using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IStatementBlock : IDom
   {
      RDomCollection<IStatementAndDetail> StatementsAll { get; }

      IEnumerable<IStatement> Statements { get; }

      bool HasBlock { get; set; }
   }
}