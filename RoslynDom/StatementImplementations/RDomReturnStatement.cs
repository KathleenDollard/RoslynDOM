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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomReturnStatement(RDomReturnStatement oldRDom)
            : base(oldRDom)
        {
            if (oldRDom.Return != null)
            { Return = oldRDom.Return.Copy(); }
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                if (Return != null)
                { list.Add(Return); }
                return list;
            }
        }

        //public override IEnumerable<IDom> Descendants
        //{ get { return Children; } }

        public IExpression Return { get; set; }
    }
}
