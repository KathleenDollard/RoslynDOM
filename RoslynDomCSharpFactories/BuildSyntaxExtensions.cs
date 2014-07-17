using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class BuildSyntaxExtensions
    {
        public static SyntaxList<AttributeListSyntax> WrapInAttributeList(this IEnumerable<SyntaxNode> attributes)
        {
            var node = SyntaxFactory.List<AttributeListSyntax>(attributes.OfType<AttributeListSyntax>()) ;
            return node;
        }

        public static  SyntaxTokenList BuildModfierSyntax(this  IHasAccessModifier hasAccessModifier)
        {
            var list = SyntaxFactory.TokenList();
            if (hasAccessModifier != null)
            { list = list.AddRange(SyntaxTokensForAccessModifier(hasAccessModifier.AccessModifier)); }
            // TODO: Static and other modifiers
            return list;
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
        public static BlockSyntax BuildStatementBlock(this IEnumerable<IStatement> statements)
        {
            var statementSyntaxList = new List<StatementSyntax>();
            foreach (var statement in statements)
            {
              //  statementSyntaxList.Add(RDomStatement statement.BuildSyntax());
            }
            //f (statementContainer.Statements.Count() == 0) { statements.Add(SyntaxFactory.EmptyStatement()); }
            var ret = SyntaxFactory.Block(statementSyntaxList);
            return ret;
        }
        //public static SyntaxList<AttributeListSyntax> BuildAttributeListSyntax(IEnumerable<IAttribute> attributes)
        //{
        //    var list = SyntaxFactory.List<AttributeListSyntax>();
        //    if (attributes.Any())
        //    {
        //        var attribList = SyntaxFactory.AttributeList();
        //        var attributeSyntax = attributes.Select(x => ((RDomAttribute)x).BuildSyntax());
        //        var attributeSyntax = attributes.Select(x => .BuildSyntax());
        //        attribList = attribList.AddAttributes(attributeSyntax.ToArray());
        //        list = list.Add(attribList);
        //    }
        //    return list;
        //}

    }
}
