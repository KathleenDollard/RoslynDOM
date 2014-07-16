using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomDeclarationStatement : RDomBase<IDeclarationStatement, ISymbol>, IDeclarationStatement
    {
        public RDomDeclarationStatement(SyntaxNode rawItem)
           : base(rawItem)
        {
            //Initialize2();
        }

        //internal RDomDeclarationStatement(
        //      VariableDeclaratorSyntax rawDeclaration,
        //      IEnumerable<PublicAnnotation> publicAnnotations)
        //    : base(rawDeclaration, publicAnnotations)
        //{
        //    Initialize();
        //}

        internal RDomDeclarationStatement(RDomDeclarationStatement oldRDom)
             : base(oldRDom)
        {
            IsImplicitlyTyped = oldRDom.IsImplicitlyTyped;
            IsConst = oldRDom.IsConst;
            Type = oldRDom.Type.Copy();
            Initializer = oldRDom.Initializer.Copy();
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    var declaration = TypedSyntax.Parent as VariableDeclarationSyntax;
        //    if (declaration == null) throw new InvalidOperationException();
        //    IsImplicitlyTyped = (declaration.Type.ToString() == "var");
        //    var typeSymbol = ((ILocalSymbol)TypedSymbol).Type;
        //    Type = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, typeSymbol);
        //    if (TypedSyntax.Initializer != null)
        //    {
        //        var equalsClause = TypedSyntax.Initializer;
        //        Initializer = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(equalsClause.Value).FirstOrDefault();
        //    }

        //}

        //protected void Initialize2()
        //{
        //    Initialize();
        //}

        //public override VariableDeclaratorSyntax BuildSyntax()
        //{
        //    return null;
        //}

        public IExpression Initializer { get; set; }

        public IReferencedType Type { get; set; }

        public bool IsImplicitlyTyped { get; set; }
        public bool IsConst { get; set; }


    }
}
