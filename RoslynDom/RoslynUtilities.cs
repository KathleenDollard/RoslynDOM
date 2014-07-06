using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using RoslynDom.Common;
using System.Reflection;

namespace RoslynDom
{
    public static class RoslynUtilities
    {
        internal static string NameFrom(this SyntaxNode node)
        {
            var qualifiedNameNode = node.ChildNodes()
                                      .OfType<QualifiedNameSyntax>()
                                      .SingleOrDefault();
            var identifierNameNodes = node.ChildNodes()
                               .OfType<IdentifierNameSyntax>();
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

        public static SyntaxTokenList SyntaxTokensForAccessModifier(AccessModifier accessModifier)
        {
            var tokenList = SyntaxFactory.TokenList();
            switch (accessModifier)
            {
                case AccessModifier.NotApplicable:
                    return tokenList;
                case AccessModifier.Private:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                case AccessModifier.ProtectedOrInternal:
                    return tokenList.AddRange(new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword) });
                case AccessModifier.Protected:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                case AccessModifier.Internal:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                case AccessModifier.Public:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                default:
                    throw new InvalidOperationException();
            }
        }

        public static SyntaxNode Format(SyntaxNode node)
        {
            node = Formatter.Format(node, new CustomWorkspace());
            return node;
        }

        public static IEnumerable<IStatement> GetStatements(BlockSyntax blockSyntax)
        {
            var list = new List<IStatement>();
            var statements = blockSyntax.Statements;
            foreach (var statement  in statements)
            {

            }
            return list;
        }

    }
}
