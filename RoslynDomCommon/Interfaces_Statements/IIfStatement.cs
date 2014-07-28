using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{


    public interface IIfStatement : IStatement, IStatementBlock, IHasCondition, IDom<IIfStatement>
    {
        RDomList<IElseStatement> Elses { get; }
        IFinalElseStatement Else { get; }
        IEnumerable<IElseIfStatement> ElseIfs { get; }

    }

    public interface IElseIfStatement : IDom<IElseIfStatement>, IElseStatement, IHasCondition
    { }

    public interface IFinalElseStatement : IDom<IFinalElseStatement>, IElseStatement
    { }
    public interface IElseStatement : IStatement, IStatementBlock
    { }
}
