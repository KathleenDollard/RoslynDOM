using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public static class RoslynCSharpUtilities
    {

        public static string NameFrom(this SyntaxNode node)
        {
            var qualifiedNameNode = node.ChildNodes()
                                      .OfType<QualifiedNameSyntax>()
                                      .SingleOrDefault();
            var identifierNameNodes = node.ChildNodes()
                               .OfType<IdentifierNameSyntax>()
                               .ToList();
            var name = "";
            if (qualifiedNameNode != null)
            {
                name = name + qualifiedNameNode.ToString();
            }
            foreach (var identifierNameNode in identifierNameNodes)
            {
                var identifierName = identifierNameNode.ToString();
                if (!(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(identifierName)))
                { name += "."; }
                name = name += identifierName;
            }
            if (!string.IsNullOrWhiteSpace(name)) return name;
            var nameToken = node.ChildTokens()
                                      .Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken)
                                      .SingleOrDefault();
            return nameToken.ValueText;
        }

        public static BlockSyntax MakeStatementBlock(IEnumerable<IStatement> statements)
        {
            var statementSyntaxList = statements
                            .SelectMany(x => RDomCSharpFactory.Factory.BuildSyntaxGroup(x))
                            .ToList();
            return SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
        }

        public static LiteralKind LiteralKindFromSyntaxKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StringLiteralToken:
                    return LiteralKind.String;
                case SyntaxKind.NumericLiteralToken:
                    return LiteralKind.Numeric;
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return LiteralKind.Boolean;
                default:
                    // I don't know how to get here, but if I get here, I want to know it :)
                    throw new NotImplementedException();
            }
        }

        public static SyntaxKind SyntaxKindFromLiteralKind(LiteralKind literalKind, object value)
        {
            switch (literalKind)
            {
                case LiteralKind.String:
                    return SyntaxKind.StringLiteralExpression;
                case LiteralKind.Numeric:
                    return SyntaxKind.NumericLiteralExpression;
                case LiteralKind.Boolean:
                    if ((bool)value) { return SyntaxKind.TrueLiteralExpression; }
                    return SyntaxKind.FalseLiteralExpression;
                case LiteralKind.Type:
                    return SyntaxKind.TypeOfExpression;
                default:
                    // I don't know how to get here, but if I get here, I want to know it :)
                    throw new NotImplementedException();
            }
        }

           public static string Simplify(SyntaxNode node)
        {
            var source = node.ToString();
            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            var solution = new CustomWorkspace().CurrentSolution
                .AddProject(projectId, "MyProject", "MyProject", LanguageNames.CSharp)
                .AddMetadataReference(projectId, RoslynRDomUtilities.Mscorlib)
                .AddMetadataReference(projectId, AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => string.Compare(a.GetName().Name, "System", StringComparison.OrdinalIgnoreCase) == 0)
                    .Select(a => new MetadataFileReference(a.Location)).Single())
                .AddDocument(documentId, "MyFile.cs", source);
            var document = solution.GetDocument(documentId);

            // Format the document.
            document = Formatter.FormatAsync(document).Result;

            // Simplify names used in the document i.e. remove unnecessary namespace qualifiers.
            var newRoot = (SyntaxNode)document.GetSyntaxRootAsync().Result;
            newRoot = new SimplifyNamesAnnotionRewriter().Visit(newRoot);
            document = document.WithSyntaxRoot(newRoot);

            document = Simplifier.ReduceAsync(document).Result;
            var ret = document.GetSyntaxRootAsync().Result.ToString();
            return ret;
        }

        public static IEnumerable<IStatementCommentWhite> GetStatementsFromSyntax(StatementSyntax statementSyntax, IDom parent, ref bool hasBlock, SemanticModel model)
        {
            var statement = RDomFactoryHelper.GetHelperForStatement().MakeItems(statementSyntax, parent, model).First();
            var list = new List<IStatementCommentWhite>();
            var blockStatement = statement as IBlockStatement;
            if (blockStatement != null)
            {
                hasBlock = true;
                foreach (var state in blockStatement.Statements)
                {
                    // Don't need to copy because abandoning block
                    list.Add(state);
                }
            }
            else
            { list.Add(statement); }
            return list;
        }

        public static StatementSyntax BuildStatement(IEnumerable<IStatement> statements, bool hasBlock)
        {
            StatementSyntax statement;
            var statementSyntaxList = statements
                         .SelectMany(x => RDomCSharpFactory.Factory.BuildSyntaxGroup(x))
                         .ToList();
            if (hasBlock || statements.Count() > 1)
            { statement = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList)); }
            else if (statements.Count() == 1)
            { statement = (StatementSyntax)statementSyntaxList.First(); }
            else
            { statement = SyntaxFactory.EmptyStatement(); }
            return statement;
        }

        private class SimplifyNamesAnnotionRewriter : CSharpSyntaxRewriter
        {
            private SyntaxNode AnnotateNodeWithSimplifyAnnotation(SyntaxNode node)
            {
                return node.WithAdditionalAnnotations(Simplifier.Annotation);
            }

            public override SyntaxNode VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
            {
                // not descending into node to simplify the whole expression
                return AnnotateNodeWithSimplifyAnnotation(node);
            }

            public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
            {
                // not descending into node to simplify the whole expression
                return AnnotateNodeWithSimplifyAnnotation(node);
            }

            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                // not descending into node to simplify the whole expression
                return AnnotateNodeWithSimplifyAnnotation(node);
            }

            public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            {
                // not descending into node to simplify the whole expression
                return AnnotateNodeWithSimplifyAnnotation(node);
            }

            public override SyntaxNode VisitGenericName(GenericNameSyntax node)
            {
                // not descending into node to simplify the whole expression
                return AnnotateNodeWithSimplifyAnnotation(node);
            }
        }

    }
}
