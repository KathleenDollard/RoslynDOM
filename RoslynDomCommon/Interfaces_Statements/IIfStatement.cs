using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IIfStatement : IDom<IIfStatement>, IStatement, IStatementBlock, IHasCondition, IElseBaseStatement
   {
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
          "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Elses",
          Justification = "I'm not sure what else to call a group of elses")]
      RDomCollection<IElseBaseStatement> Elses { get; }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
         "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Else",
        Justification = "Because this represents an else clause, it's seems an appropriate name")]
      IFinalElseStatement Else { get; }

      IEnumerable<IElseIfStatement> ElseIfs { get; }
   }

   public interface IElseIfStatement : IDom<IElseIfStatement>, IElseBaseStatement, IHasCondition
   { }

   public interface IFinalElseStatement : IDom<IFinalElseStatement>, IElseBaseStatement
   { }

   public interface IElseBaseStatement : IStatement, IStatementBlock
   { }
}