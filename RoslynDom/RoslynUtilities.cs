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

        internal static void RemoveMemberFromParent(IDom parent, IDom member)
        {
            if (member.Parent != parent) { throw new InvalidOperationException (); }
            var memberAsRDom = member as RDomBase;
            if (memberAsRDom == null) { throw new InvalidOperationException(); }
            memberAsRDom.RemoveFromParent();
        }

        internal static void PrepMemberForAdd(IDom parent, IDom member)
        {
            var memberAsRDom = member as RDomBase;
            if (memberAsRDom == null) throw new InvalidOperationException();
            if (member.Parent != null)
            {
                memberAsRDom.RemoveFromParent();
            }
            memberAsRDom.Parent = parent;
        }

        internal static IEnumerable<T> CopyMembers<T>(IEnumerable<T> members)
        {
            var ret = new List<T>();
            if (members != null)
            {
                foreach (var member in members)
                {
                    ret.Add(Copy(member));
                }
            }
            return ret;
        }

        internal static  T Copy<T>(T oldItem)
        {
            var type = oldItem.GetType();
            var constructor = type.GetTypeInfo()
                .DeclaredConstructors
                .Where(x => x.GetParameters().Count() == 1
                && typeof(T).IsAssignableFrom(x.GetParameters().First().ParameterType))
                .FirstOrDefault();
            if (constructor == null) throw new InvalidOperationException("Missing constructor for clone");
            var newItem = constructor.Invoke(new object[] { oldItem });
            return (T)newItem;
        }


        public static string MakeOuterName(string outerTypeName, string name)
        {
            return (string.IsNullOrWhiteSpace(outerTypeName) ? "" : outerTypeName + ".")
                      + name;
        }

        public static string MakeQualifiedName(string nspace, string outerTypeName, string name)
        {
            return (string.IsNullOrWhiteSpace(nspace) ? "" : nspace + ".")
                      + MakeOuterName(outerTypeName, name);
        }

        public static LiteralType LiteralTypeFromSyntaxKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StringLiteralToken:
                    return LiteralType.String;
                case SyntaxKind.NumericLiteralToken:
                    return LiteralType.Numeric;
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return LiteralType.Boolean;
                default:
                    // I don't know how to get here, but if I get here, I want to know it :)
                    throw new NotImplementedException();
            }
        }

        public static SyntaxKind SyntaxKindFromLiteralType(LiteralType literalType, object value)
        {
            switch (literalType)
            {
                case LiteralType.String:
                    return SyntaxKind.StringLiteralExpression;
                case LiteralType.Numeric:
                    return SyntaxKind.NumericLiteralExpression;
                case LiteralType.Boolean:
                    if ((bool)value) { return SyntaxKind.TrueLiteralExpression; }
                    return SyntaxKind.FalseLiteralExpression;
                case LiteralType.Type:
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
    }
}
