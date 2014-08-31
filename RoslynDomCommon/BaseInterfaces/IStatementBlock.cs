using System.Collections.Generic;

namespace RoslynDom.Common
{
     public interface IStatementBlock : IDom
    {
        RDomCollection<IStatementCommentWhite> StatementsAll { get; }
        IEnumerable <IStatement> Statements { get; }
        bool HasBlock { get; set; }
    }
}
