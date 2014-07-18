using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomWhileStatement : RDomBaseLoop<IWhileStatement>, IWhileStatement
    {
        //private IList<IStatement> _statements = new List<IStatement>();

        public RDomWhileStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomWhileStatement(RDomWhileStatement oldRDom)
            : base(oldRDom)
        {

            //var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            //foreach (var statement in statements)
            //{ AddOrMoveStatement(statement); }
            //Condition = oldRDom.Condition.Copy();
            //HasBlock = oldRDom.HasBlock;
            //TestAtEnd = oldRDom.TestAtEnd;
        }

        //public IExpression Condition { get; set; }

        //public bool HasBlock { get; set; }
        //public bool TestAtEnd { get; set; }

        //public void RemoveStatement(IStatement statement)
        //{ _statements.Remove(statement); }

        //public void AddOrMoveStatement(IStatement statement)
        //{ _statements.Add(statement); }

        //public IEnumerable<IStatement> Statements
        //{ get { return _statements; } }

    }
}
