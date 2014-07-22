using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAssignmentStatement : RDomBase<IAssignmentStatement, ISymbol>, IAssignmentStatement
    {

        public RDomAssignmentStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomAssignmentStatement(RDomAssignmentStatement oldRDom)
             : base(oldRDom)
        {
            Name = oldRDom.Name;
            Expression = oldRDom.Expression;
        }

        public override IEnumerable<IDom> Children
        { get { return new List<IDom>() { Expression }; } }

        public override IEnumerable<IDom> Descendants
        { get { return new List<IDom>() { Expression }; } }

        public string Name { get; set; }
        public IExpression Expression { get; set; }
    }
}
