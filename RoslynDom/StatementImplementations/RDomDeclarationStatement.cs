using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomDeclarationStatementFactory
        : RDomStatementFactory<RDomDeclarationStatement, VariableDeclaratorSyntax>
    {
        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is LocalDeclarationStatementSyntax;
        }

        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode)
        {
            var list = new List<IStatement>();
            // We can't do this in the constructor, because many may be created and we want to flatten
            var rawDeclaration = syntaxNode as LocalDeclarationStatementSyntax;
            var rawVariableDeclaration = rawDeclaration.Declaration;
            var declarators = rawDeclaration.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomDeclarationStatement( decl);
                list.Add(newItem);
                InitializeItem(newItem, decl);
            }
            return list;
        }

        public override void InitializeItem(RDomDeclarationStatement newItem, VariableDeclaratorSyntax syntax)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            var declaration = syntax.Parent as VariableDeclarationSyntax;
            if (declaration == null) throw new InvalidOperationException();
            newItem.IsImplicitlyTyped = (declaration.Type.ToString() == "var");
            var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type;
            newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, typeSymbol);
            if (syntax.Initializer != null)
            {
                var equalsClause = syntax.Initializer;
                newItem.Initializer = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(equalsClause.Value).FirstOrDefault();
            }

        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IDeclarationStatement;
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            TypeSyntax typeSyntax;
            // TODO: Type alias are not currently being used. Could be brute forced here, but I'd rather run a simplifier for real world scenarios
            if (itemAsT.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDomFactory.BuildSyntax(itemAsT.Type)); }
            var nodeDeclarator = SyntaxFactory.VariableDeclarator(item.Name);
            if (itemAsT.Initializer != null)
            {
                var expressionSyntax = RDomFactory.BuildSyntax(itemAsT.Initializer);
                nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
            }
            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            var node = SyntaxFactory.LocalDeclarationStatement(nodeDeclaration);
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }


    public class RDomDeclarationStatement : RDomBase<IDeclarationStatement, VariableDeclaratorSyntax, ISymbol>, IDeclarationStatement
    {
        internal RDomDeclarationStatement(VariableDeclaratorSyntax rawItem)
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
