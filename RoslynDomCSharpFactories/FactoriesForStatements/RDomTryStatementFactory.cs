using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomTryStatementFactory
         : RDomStatementFactory<RDomTryStatement, TryStatementSyntax>
    {
        public RDomTryStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as TryStatementSyntax;
            var newItem = new RDomTryStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Block, newItem, model);
            var catchSyntaxList = syntax.ChildNodes()
                                    .Where(x => x.CSharpKind() == SyntaxKind.CatchClause)
                                    .OfType<CatchClauseSyntax>();
            foreach (var ctch in catchSyntaxList)
            {
                var newCatch = new RDomCatchStatement(ctch, newItem, model);
                CreateFromWorker.StandardInitialize(newCatch, ctch, newItem, model);
                CreateFromWorker.InitializeStatements(newCatch, ctch.Block, newCatch, model);
                if (ctch.Declaration != null)
                {
                    var variable = GetDeclaration(newCatch, ctch, model);
                    newCatch.Variable = variable;
                    var typeSymbol = model.GetTypeInfo(ctch.Declaration.Type).Type;
                    newCatch.ExceptionType = new RDomReferencedType(typeSymbol.DeclaringSyntaxReferences, typeSymbol);
                }
                if (ctch.Filter != null)
                { newCatch.Condition = Corporation.CreateFrom<IExpression>(ctch.Filter.FilterExpression, newCatch, model).FirstOrDefault(); }
                newItem.CatchesAll.AddOrMove(newCatch);
            }
            if (syntax.Finally != null)
            {
                var newFinally = new RDomFinallyStatement(syntax.Finally, newItem, model);
                CreateFromWorker.StandardInitialize(newFinally, syntax.Finally, parent, model);
                CreateFromWorker.InitializeStatements(newFinally, syntax.Finally.Block, newFinally, model);
                newItem.Finally = newFinally;
            }
            return newItem;
        }

        private IVariableDeclaration GetDeclaration(RDomCatchStatement newCatch,
            CatchClauseSyntax ctch, SemanticModel model)
        {
            if (string.IsNullOrWhiteSpace(ctch.Declaration.Identifier.ToString())) return null;
            ISymbol typeSymbol = model.GetDeclaredSymbol(ctch.Declaration);
            if (typeSymbol == null)
            { typeSymbol = model.GetTypeInfo(ctch.Declaration.Type).Type; }
            // TODO: Reconsider Symbol being write only or have an overridable way to retrieve
            // newCatch.Symbol = typeSymbol;
            var variable = Corporation.CreateFrom<IMisc>(ctch.Declaration, newCatch, model).FirstOrDefault() as IVariableDeclaration;
            return variable;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as ITryStatement;
            var node = SyntaxFactory.TryStatement();
            var catches = BuildCatchSyntaxList(itemAsT);
            var fnally = BuildFinallySyntax(itemAsT);
            var block = BuildSyntaxWorker.GetStatementBlock(itemAsT.Statements);

            node = node.WithCatches(SyntaxFactory.List(catches))
                     .WithFinally(fnally)
                     .WithBlock(block);

            return item.PrepareForBuildSyntaxOutput(node);
        }

        private FinallyClauseSyntax BuildFinallySyntax(ITryStatement itemAsT)
        {
            var fnally = itemAsT.Finally;
            // TODO: Empty statement would return empty brackets here?
            var block = BuildSyntaxWorker.GetStatementBlock(fnally.Statements);
            var syntax = SyntaxFactory.FinallyClause(block);
            return syntax;
        }

        private IEnumerable<CatchClauseSyntax> BuildCatchSyntaxList(ITryStatement itemAsT)
        {
            var ret = new List<CatchClauseSyntax>();
            foreach (var ctch in itemAsT.Catches)
            {
                var syntax = SyntaxFactory.CatchClause();
                if (ctch.ExceptionType != null)
                {
                    TypeSyntax typeSyntax = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(ctch.ExceptionType));
                    var declaration = SyntaxFactory.CatchDeclaration(typeSyntax);
                    if (ctch.Variable != null)
                    { declaration = declaration.WithIdentifier(SyntaxFactory.Identifier(ctch.Variable.Name)); }
                }
                // TODO: Add catch filter for 6.0
                // TODO: Empty statement would return empty brackets here?
                var block = BuildSyntaxWorker.GetStatementBlock(ctch.Statements);
                syntax = syntax.WithBlock(block);
                ret.Add(syntax);
            }

            return ret;
        }
    }
}
