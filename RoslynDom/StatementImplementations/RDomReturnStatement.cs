using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReturnStatement : RDomBase<IReturnStatement, ISymbol>, IReturnStatement
    {

        public RDomReturnStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomReturnStatement(RDomReturnStatement oldRDom)
            : base(oldRDom)
        {
            Return = oldRDom.Return.Copy();
        }

        public override IEnumerable<IDom> Children
        { get { return new List<IDom>() { Return }; } }

        public override IEnumerable<IDom> Descendants
        { get { return new List<IDom>() { Return }; } }

        public IExpression Return { get; set; }
    }
}
