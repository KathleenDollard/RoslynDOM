using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public static class BuildSyntaxHelpers
    {
        // it doesn't feel like this belongs here, but until the design of PrepareForBuildSyntaxOutput solidifies, leaving it
        private static TriviaManager triviaManager = new TriviaManager();

        [ExcludeFromCodeCoverage]
        private static string nameof<T>(T value) { return ""; }

        private const string annotationMarker = "RDomBuildSyntaxMarker";

        public static IEnumerable<SyntaxNode> PrepareForBuildSyntaxOutput(this IEnumerable<SyntaxNode> nodes, IDom item)
        {
            var ret = new List<SyntaxNode>();
            foreach (var node in nodes)
            {
                ret.Add(PrepareForBuildItemSyntaxOutput(node, item));
            }
            return ret;
        }

        public static IEnumerable<SyntaxNode> PrepareForBuildSyntaxOutput(this SyntaxNode node, IDom item)
        {
            node = PrepareForBuildItemSyntaxOutput(node, item);
            return new SyntaxNode[] { node };
        }

        public static TNode PrepareForBuildItemSyntaxOutput<TNode>(this TNode node, IDom item)
            where TNode : SyntaxNode
        {
            var moreLeadingTrivia = BuildSyntaxHelpers.LeadingTrivia(item);
            var leadingTriviaList = moreLeadingTrivia.Concat(node.GetLeadingTrivia());
            node = node.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTriviaList));

            return node;
        }

        public static SyntaxList<AttributeListSyntax> WrapInAttributeList(IEnumerable<SyntaxNode> attributes)
        {
            var node = SyntaxFactory.List<AttributeListSyntax>(attributes.OfType<AttributeListSyntax>());
            return node;
        }

        public static SyntaxTokenList BuildModfierSyntax(this IDom item)
        {
            var list = new List<SyntaxToken>();

            var hasAccessModifier = item as IHasAccessModifier;
            if (hasAccessModifier != null && hasAccessModifier.DeclaredAccessModifier != AccessModifier.None)
            { list.AddRange(SyntaxTokensForAccessModifier(hasAccessModifier.DeclaredAccessModifier)); }

            var canBeStatic = item as ICanBeStatic;
            if (canBeStatic != null && canBeStatic.IsStatic)
            { list.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword)); }

            var canBeNew = item as ICanBeNew;
            if (canBeNew != null && canBeNew.IsNew)
            { list.Add(SyntaxFactory.Token(SyntaxKind.NewKeyword)); }

            var supportsOO = item as IOOTypeMember;
            if (supportsOO != null)
            {
                if (supportsOO.IsAbstract) { list.Add(SyntaxFactory.Token(SyntaxKind.AbstractKeyword)); }
                if (supportsOO.IsOverride) { list.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword)); }
                if (supportsOO.IsVirtual) { list.Add(SyntaxFactory.Token(SyntaxKind.VirtualKeyword)); }
                if (supportsOO.IsSealed) { list.Add(SyntaxFactory.Token(SyntaxKind.SealedKeyword)); }
            }

            return SyntaxFactory.TokenList(list);
        }

        public static SyntaxTriviaList LeadingTrivia(IDom item)
        {
            var leadingTrivia = new List<SyntaxTrivia>();
            leadingTrivia.AddRange(BuildCommentWhite(item));
            leadingTrivia.AddRange(BuildSyntaxHelpers.BuildStructuredDocumentationSyntax(item as IHasStructuredDocumentation));
            return SyntaxFactory.TriviaList(leadingTrivia);
        }


        private static IEnumerable<SyntaxTrivia> BuildCommentWhite(IDom item)
        {
            var ret = new List<SyntaxTrivia>();
            // This can happen if someone copies an item to a new item, does not attach it to a tree, 
            // and asks for the syntax. It's actually expected to sometimes be unattached. 
            if (item.Parent == null) { return ret; }
            if (TryBuildCommentWhiteFor<IStemMemberCommentWhite, IStemContainer>(item, ret, x => x.StemMembersAll)) { return ret; }
            if (TryBuildCommentWhiteFor<ITypeMemberCommentWhite, ITypeMemberContainer>(item, ret, x => x.MembersAll)) { return ret; }
            if (TryBuildCommentWhiteFor<IStatementCommentWhite, IStatementContainer>(item, ret, x => x.StatementsAll)) { return ret; }
            return ret;
        }

        private static bool TryBuildCommentWhiteFor<TKind, TParent>
                    (IDom item, List<SyntaxTrivia> trivias, Func<TParent, IEnumerable<TKind>> getCandidates)
            where TParent : class
            where TKind : class
        {
            var itemAsTKind = item as TKind;
            if (itemAsTKind == null) { return false; }
            // if item is TKind, parent may not be TParent because types can be multiply rooted (stem or nested type)
            var parentAsTParent = item.Parent as TParent;
            if (parentAsTParent == null) return false;
            var candidates = getCandidates(parentAsTParent);
            var commentWhites = candidates
                                .PreviousSiblingsUntil(itemAsTKind, x => !(x is IComment || x is IVerticalWhitespace))
                                .OfType<ICommentWhite>();
            trivias.AddRange(MakeWhiteCommentTrivia(commentWhites));
            return true;
        }

        private static IEnumerable<SyntaxTrivia> MakeWhiteCommentTrivia(IEnumerable<ICommentWhite> commentWhites)
        {
            var ret = new List<SyntaxTrivia>();
            foreach (var item in commentWhites)
            {
                if (item is IVerticalWhitespace) { ret.Add(SyntaxFactory.EndOfLine("\r\n")); }
                else
                {
                    var itemAsComment = item as IComment;
                    Guardian.Assert.IsNotNull(itemAsComment, nameof(itemAsComment));
                    var innerWs = itemAsComment.Whitespace2Set[LanguagePart.Inner, LanguageElement.Comment];
                    var comment = innerWs.LeadingWhitespace + itemAsComment.Text + innerWs.TrailingWhitespace;
                    if (itemAsComment.IsMultiline) { comment = "/*" + comment + "*/"; }
                    else { comment = "//" + comment; }
                    var commentSyntax = SyntaxFactory.Comment(comment);

                    // Assume just one whitespace
                    var whitespace = itemAsComment.Whitespace2Set.FirstOrDefault();
                    if (whitespace != null)
                    {
                        // for now assume only whitespace before and newline after
                        ret.Add(SyntaxFactory.Whitespace(whitespace.LeadingWhitespace));
                        ret.Add(commentSyntax);
                    }
                    else
                    { ret.Add(commentSyntax); }
                    ret.Add(SyntaxFactory.EndOfLine("\r\n"));
                }
            }
            return ret;
        }

        internal static SyntaxToken GetTokenFromKind(LiteralKind kind, object value)
        {
            switch (kind)
            {
            case LiteralKind.Constant:
            case LiteralKind.String:
            case LiteralKind.Unknown:
                return SyntaxFactory.Literal(value.ToString());
            case LiteralKind.Numeric:
                if (GeneralUtilities.IsInteger(value))
                { return SyntaxFactory.Literal(Convert.ToInt32(value)); }
                if (GeneralUtilities.IsFloatingPint(value))
                { return SyntaxFactory.Literal(Convert.ToDouble(value)); }
                if (value is uint)
                { return SyntaxFactory.Literal(Convert.ToUInt32(value)); }
                if (value is long)
                { return SyntaxFactory.Literal(Convert.ToInt64(value)); }
                if (value is ulong)
                { return SyntaxFactory.Literal(Convert.ToUInt64(value)); }
                else
                { return SyntaxFactory.Literal(Convert.ToDecimal(value)); }
            case LiteralKind.Boolean:
            case LiteralKind.Type:
            // Need to create an expression so handled separately and should not call this
            default:
                break;
            }
            throw new NotImplementedException();
        }

        public static IEnumerable<SyntaxTrivia> BuildStructuredDocumentationSyntax(IHasStructuredDocumentation itemHasStructDoc)
        {
            var ret = new List<SyntaxTrivia>();
            if (itemHasStructDoc == null ||
                (itemHasStructDoc.StructuredDocumentation.Document == null
                && string.IsNullOrEmpty(itemHasStructDoc.Description)))
            { return ret; }
            var itemStructDoc = itemHasStructDoc.StructuredDocumentation;
            var leadingWs = "";
            var innerLeadingWs = " ";
            XDocument xDoc = null;

            if (itemHasStructDoc.StructuredDocumentation == null)
            {
                xDoc = new XDocument();
            }
            else
            {
                xDoc = XDocument.Parse(itemHasStructDoc.StructuredDocumentation.Document);
                leadingWs = itemStructDoc.Whitespace2Set[LanguageElement.DocumentationComment].LeadingWhitespace;
                innerLeadingWs = itemStructDoc.Whitespace2Set[LanguagePart.Inner, LanguageElement.DocumentationComment].LeadingWhitespace;
            }
            var description = "\r\n" + innerLeadingWs + itemHasStructDoc.Description + "\r\n";

            SetDescriptionInXDoc(xDoc, description);
            string newDocAsString = PrefixLinesWithDocCommentPrefix(leadingWs, xDoc);

            var triviaList = SyntaxFactory.ParseLeadingTrivia(newDocAsString);
            if (triviaList.Any())
            { return triviaList; }
            return ret;
        }

        private static void SetDescriptionInXDoc(XDocument xDoc, string description)
        {
            var oldParent = xDoc.DescendantNodes().OfType<XElement>().Where(x => x.Name == "member").FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(description))
            {
                var oldSummaryElement = xDoc
                            .DescendantNodes().OfType<XElement>()
                            .Where(x => x.Name == "summary")
                            .FirstOrDefault();
                if (oldSummaryElement != null)
                { oldSummaryElement.Value = description; }
                else
                {
                    var newSummary = new XElement("summary", description);
                    oldParent.AddFirst(newSummary);
                }
            }
        }

        private static string PrefixLinesWithDocCommentPrefix(string leadingWs, XDocument xDoc)
        {
            // No doubt I'll feel dirty in the morning, but the manual alternative is awful
            var oldDocsAsString = xDoc.ToString();
            var lines = oldDocsAsString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var newDocAsString = ""; // these are short, so decided not to use StringBuilder
            foreach (var line in lines)
            {
                var useLine = line.Trim();
                if (useLine.StartsWith("<member") || useLine.StartsWith("</member")) continue;
                newDocAsString += leadingWs + "/// " + useLine + "\r\n";
            }

            return newDocAsString;
        }

        public static SyntaxTokenList SyntaxTokensForAccessModifier(AccessModifier accessModifier)
        {
            var tokenList = SyntaxFactory.TokenList();
            switch (accessModifier)
            {
            case AccessModifier.None:
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

        public static BaseListSyntax GetBaseList(IHasImplementedInterfaces item)
        {
            var list = new List<TypeSyntax>();
            var asClass = item as IClass;
            if (asClass != null)
            {
                if (asClass.BaseType != null)
                {
                    var baseTypeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntax(asClass.BaseType);
                    list.Add(baseTypeSyntax);
                }
            }
            foreach (var interf in item.ImplementedInterfaces)
            {
                var interfTypeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntax(interf);
                list.Add(interfTypeSyntax);
            }

            var colonToken = SyntaxFactory.Token(SyntaxKind.ColonToken);
            colonToken = BuildSyntaxHelpers.AttachWhitespaceToToken(colonToken, item.Whitespace2Set[LanguageElement.BaseListPrefix]);

            return list.Any()
                     ? SyntaxFactory.BaseList(colonToken, SyntaxFactory.SeparatedList(list))
                     : null;
        }

        public static TypeParameterListSyntax GetTypeParameterSyntaxList(
                 IEnumerable<SyntaxNode> typeParamsAndConstraints,
                 Whitespace2Collection whitespace2Set,
                 WhitespaceKindLookup whitespaceLookup)
        {
            var typeParameters = typeParamsAndConstraints
                            .OfType<TypeParameterSyntax>()
                            .ToList();
            if (typeParameters.Any())
            {
                var typeParameterListSyntax = SyntaxFactory.TypeParameterList(
                    SyntaxFactory.SeparatedList<TypeParameterSyntax>(typeParameters));
                typeParameterListSyntax = AttachWhitespace(
                            typeParameterListSyntax, whitespace2Set,
                            whitespaceLookup);
                return typeParameterListSyntax; ;
            }
            return null;
        }

        public static SyntaxList<TypeParameterConstraintClauseSyntax> GetTypeParameterConstraintList(
                 IEnumerable<SyntaxNode> typeParamsAndConstraints,
                 Whitespace2Collection whitespace2Set,
                 WhitespaceKindLookup whitespaceLookup)
        {
            var typeParameters = typeParamsAndConstraints
                           .OfType<TypeParameterSyntax>()
                           .ToList();
            var typeConstraintClauses = typeParamsAndConstraints
                    .OfType<TypeParameterConstraintClauseSyntax>()
                    .ToList();
            var clauses = new List<TypeParameterConstraintClauseSyntax>();
            foreach (var typeParameter in typeParameters)
            {
                var name = typeParameter.Identifier.ToString();
                var constraint = typeConstraintClauses
                              .Where(x => x.Name.ToString() == name
                                          && x.Constraints.Any())
                              .ToList()
                              .SingleOrDefault();
                if (constraint != null)
                { clauses.Add(constraint); }
            }
            return SyntaxFactory.List(clauses);
        }

        public static ExpressionSyntax BuildArgValueExpression(object value, string declaredConst, LiteralKind valueType)
        {
            var kind = Mappings.SyntaxKindFromLiteralKind(valueType, value);
            ExpressionSyntax expr = null;
         if (valueType == LiteralKind.Boolean)
         { expr = SyntaxFactory.LiteralExpression(kind); }
         else if (valueType == LiteralKind.Type)
         {
            var type = value as RDomReferencedType;
            if (type == null) throw new InvalidOperationException();
            var typeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(type).First();
            expr = SyntaxFactory.TypeOfExpression(typeSyntax);
         }
         else if (valueType == LiteralKind.Constant)
         {
            var leftExpr = SyntaxFactory.IdentifierName(declaredConst.SubstringBeforeLast("."));
            var name = SyntaxFactory.IdentifierName(declaredConst.SubstringAfterLast("."));
            expr = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, leftExpr, name);
         }
         else
            {
                var token = BuildSyntaxHelpers.GetTokenFromKind(valueType, value);
                expr = SyntaxFactory.LiteralExpression((SyntaxKind)kind, token);
            }

            return expr;
        }

        public static T AttachWhitespace<T>(T syntaxNode, Whitespace2Collection whitespace2Set, WhitespaceKindLookup whitespaceLookup)
               where T : SyntaxNode
        {
            return triviaManager.AttachWhitespace(syntaxNode, whitespace2Set, whitespaceLookup);
        }

        public static T AttachWhitespace<T>(T syntaxNode, Whitespace2Collection whitespace2Set, WhitespaceKindLookup whitespaceLookup, LanguagePart languagePart)
               where T : SyntaxNode
        {
            return triviaManager.AttachWhitespace(syntaxNode, whitespace2Set, whitespaceLookup, languagePart);
        }

        public static T AttachWhitespaceToFirst<T>(T syntaxNode, Whitespace2 whitespace2)
            where T : SyntaxNode
        {
            if (whitespace2 == null) return syntaxNode;
            return triviaManager.AttachWhitespaceToFirst(syntaxNode, whitespace2);
        }

        public static T AttachWhitespaceToLast<T>(T syntaxNode, Whitespace2 whitespace2)
                 where T : SyntaxNode
        {
            return triviaManager.AttachWhitespaceToLast(syntaxNode, whitespace2);
        }

        public static T AttachWhitespaceToFirstAndLast<T>(T syntaxNode, Whitespace2 whitespace2)
         where T : SyntaxNode
        {
            return triviaManager.AttachWhitespaceToFirstAndLast(syntaxNode, whitespace2);
        }
        internal static SyntaxToken AttachWhitespaceToToken(SyntaxToken token, Whitespace2 whitespace2)
        {
            return triviaManager.AttachWhitespaceToToken(token, whitespace2);
        }
    }
}
