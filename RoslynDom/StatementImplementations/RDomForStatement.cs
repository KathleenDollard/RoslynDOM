using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomForStatement : RDomBaseLoop<IForStatement>, IForStatement
    {

        public RDomForStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomForStatement(RDomForStatement oldRDom)
            : base(oldRDom)
        {
            Incrementor = oldRDom.Incrementor.Copy();
            Variable = oldRDom.Variable.Copy();
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                list.Add(Incrementor);
                list.Add(Variable);
                list.AddRange(base.Children);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = new List<IDom>() ;
                list.Add(Incrementor);
                list.AddRange(Variable.DescendantsAndSelf);
                list.AddRange(base.Descendants);
                return list;
            }
        }

        public IExpression Incrementor { get; set; }
      

        public IVariableDeclaration Variable { get; set; }
   
    }
}
