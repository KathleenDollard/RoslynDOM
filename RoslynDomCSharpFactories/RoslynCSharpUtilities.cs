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
                            .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                            .ToList();
            return SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
        }

        public static StatementSyntax BuildStatement(IEnumerable<IStatement> statements, 
                IStatementBlock parent, WhitespaceKindLookup whitespaceLookup)
        {
            StatementSyntax statementBlock;
            var statementSyntaxList = statements
                         .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                         .ToList();
            var hasBlock = parent.HasBlock;
            if (hasBlock || statements.Count() > 1)
            {
                statementBlock = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
                statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
                // Block tokens are held in parent
            }
            else if (statements.Count() == 1)
            {
                statementBlock = (StatementSyntax)statementSyntaxList.First();
                //statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
            }
            else
            {
                statementBlock = SyntaxFactory.EmptyStatement();
                statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
            }
            return statementBlock;
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
