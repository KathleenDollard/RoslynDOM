using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomForEachStatement : RDomBaseLoop<IForEachStatement>, IForEachStatement
    {

        public RDomForEachStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomForEachStatement(RDomForEachStatement oldRDom)
            : base(oldRDom)
        {
            Variable = oldRDom.Variable;
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                list.Add(Variable);
                list.AddRange(base.Children);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = new List<IDom>();
                list.AddRange(Variable.DescendantsAndSelf);
                list.AddRange(base.Descendants);
                return list;
            }
        }

        public IVariableDeclaration Variable { get;set; }
          }
}
