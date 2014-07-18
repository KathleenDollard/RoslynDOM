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

        public IExpression Initializer { get; set; }

        public IReferencedType Type { get; set; }

        public bool IsImplicitlyTyped { get; set; }
        public bool IsConst { get; set; }


    }
}
