using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{


    public interface IIfStatement : IDom<IIfStatement>, IStatement, IStatementBlock, IHasCondition, IElseBaseStatement
    {
        RDomList<IElseBaseStatement> Elses { get; }
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
