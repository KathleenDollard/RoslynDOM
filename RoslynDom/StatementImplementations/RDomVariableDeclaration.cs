using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomVariableDeclaration : RDomBase<IVariableDeclaration, ISymbol>, IVariableDeclaration
    {
        public RDomVariableDeclaration(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomVariableDeclaration(IVariableDeclaration oldRDom)
             : base(oldRDom)
        {
            IsImplicitlyTyped = oldRDom.IsImplicitlyTyped;
            IsConst = oldRDom.IsConst;
            Type = oldRDom.Type.Copy();
            Initializer = oldRDom.Initializer.Copy();
        }

        public override IEnumerable<IDom> Children
        { get { return new List<IDom>() { Initializer }; } }

        public override IEnumerable<IDom> Descendants
        { get { return new List<IDom>() { Initializer }; } }

        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }


        public IExpression Initializer { get; set; }

        public IReferencedType Type { get; set; }

        public bool IsImplicitlyTyped { get; set; }
        public bool IsConst { get; set; }


    }
}
