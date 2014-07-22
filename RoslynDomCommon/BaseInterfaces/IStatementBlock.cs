using System.Collections.Generic;

namespace RoslynDom.Common
{
     public interface IStatementBlock : IDom
    {
        RDomList<IStatement> Statements { get; }
        bool HasBlock { get; set; }
    }
}
