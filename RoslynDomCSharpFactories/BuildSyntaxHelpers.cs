using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

//using Microsoft.CodeAnalysis.Formatting;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoslynDom.CSharp
{
   public static class BuildSyntaxHelpers
   {
      // it doesn't feel like this belongs here, but until the design of PrepareForBuildSyntaxOutput solidifies, leaving it
      private static TriviaManager triviaManager = new TriviaManager();

      [ExcludeFromCodeCoverage]
      private static string nameof<T>(T value) { return ""; }

      private const string annotationMarker = "RDomBuildSyntaxMarker";

      public static IEnumerable<SyntaxNode> PrepareForBuildSyntaxOutput(this IEnumerable<SyntaxNode> nodes, IDom item, OutputContext context)
      {
         var ret = new List<SyntaxNode>();
         foreach (var node in nodes)
         {
            ret.Add(PrepareForBuildItemSyntaxOutput(node, item, context));
         }
         return ret;
      }

      public static IEnumerable<SyntaxNode> PrepareForBuildSyntaxOutput(this SyntaxNode node, IDom item, OutputContext context)
      {
         node = PrepareForBuildItemSyntaxOutput(node, item, context);
         return new SyntaxNode[] { node };
      }

      public static TNode PrepareForBuildItemSyntaxOutput<TNode>(this TNode node, IDom item, OutputContext context)
          where TNode : SyntaxNode
      {
         var leadingTriviaList = LeadingTrivia(item, context).Concat(node.GetLeadingTrivia());
         node = node.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTriviaList));
         var trailingTriviaList = node.GetTrailingTrivia().Concat(TrailingTrivia(item, context));
         node = node.WithTrailingTrivia(SyntaxFactory.TriviaList(trailingTriviaList));

         if (item.NeedsFormatting)
         { node = (TNode)RDom.CSharp.Format(node); }
         return node;
      }

      private static SyntaxTriviaList BuildTriviaList(IEnumerable<SyntaxTrivia> newTrivia, SyntaxTriviaList oldTrivia)
      {
         var fullTrivia = oldTrivia.Concat(newTrivia);
         return SyntaxFactory.TriviaList(fullTrivia);
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

      internal static T RemoveLeadingSpaces<T>(T syntax)
         where T : SyntaxNode
      {
         var trivia = syntax.GetLeadingTrivia();
         var newTrivia = trivia.Where(x => x.CSharpKind() != SyntaxKind.WhitespaceTrivia);
         syntax = syntax.WithLeadingTrivia(newTrivia);
         return syntax;
      }

      public static IEnumerable<SyntaxTrivia> LeadingTrivia(IDom item, OutputContext context)
      {
         var leadingTrivia = new List<SyntaxTrivia>();
         // These are different because StructuredDocs is attached in RoslynDom
         leadingTrivia.AddRange(BuildPreviousDetail(item, context));
         leadingTrivia.AddRange(BuildStructuredDocumentationSyntax(item as IHasStructuredDocumentation, context));
         return leadingTrivia;
      }

      public static IEnumerable<SyntaxTrivia> TrailingTrivia(IDom item, OutputContext context)
      {
         if (item == null) { return new List<SyntaxTrivia>(); }
         var parentAsContainer = item.Parent as IContainer;
         if (parentAsContainer == null) return new List<SyntaxTrivia>();
         var parentMembers = parentAsContainer.GetMembers();
         if (!ShouldOutputTrailing(item, parentMembers)) return new List<SyntaxTrivia>();
         var details = parentMembers
                        .FollowingSiblingsUntil(item, x => !(x is IDetail))
                        .OfType<IDetail>();
         return BuildDetails(context, details);
      }

      public static bool ShouldOutputTrailing(IDom item, IEnumerable<IDom> parentMembers)
      {
         var lastOrDefault = parentMembers
                                    .Where(x => !(x is IDetail))
                                    .LastOrDefault();
         // TODO: There are no non detail members on the parent. TODO is to test this scenario, which might not even get this far
         if (lastOrDefault == null) return true;
         return (item as IDom) == lastOrDefault;
      }

      private static IEnumerable<SyntaxTrivia> BuildPreviousDetail<T>(T item, OutputContext context)
         where T : IDom
      {
         if (item == null) { return new List<SyntaxTrivia>(); }
         var parentAsContainer = item.Parent as IContainer;
         if (parentAsContainer == null) return new List<SyntaxTrivia>();
         var parentMembers = parentAsContainer.GetMembers();
         var details = parentMembers
                            .PreviousSiblingsUntil(item, x => !(x is IDetail))
                            .OfType<IDetail>();

         return BuildDetails(context, details);
      }

      private static IEnumerable<SyntaxTrivia> BuildDetails(OutputContext context, IEnumerable<IDetail> details)
      {
         var trivias = new List<SyntaxTrivia>();
         foreach (var detail in details)
         {
            if (detail is IVerticalWhitespace)
            { trivias.Add(SyntaxFactory.EndOfLine("\r\n")); }
            else
            {
               ITriviaFactory factory = GetFactory<IDetail>(context, detail);
               if (factory == null) factory = GetFactory<IStructuredDocumentation>(context, detail);
               if (factory == null) factory = GetFactory<IPublicAnnotation>(context, detail);
               trivias.AddRange(factory.BuildSyntaxTrivia(detail, context));
            }
         }
         return trivias;
      }

      private static ITriviaFactory<T> GetFactory<T>(OutputContext context, IDetail item)
      where T : class
      {
         var itemAsT = item as T;
         if (itemAsT != null)
         {
            return context.Corporation.GetTriviaFactory<T>();
         }
         return null;
      }

      internal static SyntaxToken GetTokenFromKind(LiteralKind kind, object value)
      {
         switch (kind)
         {
            case LiteralKind.MemberAccess:
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
            case LiteralKind.Default:
            // Need to create an expression so handled separately and should not call this
            default:
               break;
         }
         throw new NotImplementedException();
      }

      public static IEnumerable<SyntaxTrivia> BuildStructuredDocumentationSyntax(IHasStructuredDocumentation itemAsT, OutputContext context)
      {
         if (itemAsT == null) return new List<SyntaxTrivia>();
         var factory = context.Corporation.GetTriviaFactory<IStructuredDocumentation>();
         return factory.BuildSyntaxTrivia(itemAsT.StructuredDocumentation, context);
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
         var list = new List<BaseTypeSyntax>();
         var asClass = item as IClass;
         if (asClass != null)
         {
            if (asClass.BaseType != null)
            {
               var baseTypeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxNode(asClass.BaseType);
               var baseSyntax = SyntaxFactory.SimpleBaseType(baseTypeSyntax);
               list.Add(baseSyntax);
            }
         }
         foreach (var interf in item.ImplementedInterfaces)
         {
            var interfTypeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxNode(interf);
            var baseSyntax = SyntaxFactory.SimpleBaseType(interfTypeSyntax);
            list.Add(baseSyntax);
         }

         var colonToken = SyntaxFactory.Token(SyntaxKind.ColonToken);
         colonToken = BuildSyntaxHelpers.AttachWhitespaceToToken(colonToken, item.Whitespace2Set[LanguageElement.BaseListPrefix]);

         return list.Any()
                  ? SyntaxFactory.BaseList(colonToken, SyntaxFactory.SeparatedList<BaseTypeSyntax>(list))
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
         else if (valueType == LiteralKind.Null)
         {
            expr = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
         }
         else if (valueType == LiteralKind.Type)
         {
            var type = value as RDomReferencedType;
            if (type == null) throw new InvalidOperationException();
            var typeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(type).First();
            expr = SyntaxFactory.TypeOfExpression(typeSyntax);
         }
         else if (valueType == LiteralKind.Default)
         {
            var type = value as RDomReferencedType;
            if (type == null) throw new InvalidOperationException();
            var typeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(type).First();
            expr = SyntaxFactory.DefaultExpression(typeSyntax);
         }
         else if (valueType == LiteralKind.MemberAccess)
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

      public static TSyntax BuildTypeParameterSyntax<TSyntax>(IHasTypeParameters itemAsT, TSyntax node,
               WhitespaceKindLookup whitespaceLookup,
               Func<TSyntax, TypeParameterListSyntax, TSyntax> addTypeParameters,
               Func<TSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>, TSyntax> addTypeParameterConstraints)
         where TSyntax : SyntaxNode
      {
         // This works oddly because it uncollapses the list
         // This code is largely repeated in interface and class factories, but is very hard to refactor because of shallow Roslyn (Microsoft) architecture
         var typeParamsAndConstraints = itemAsT.TypeParameters
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .ToList();

         var typeParameterSyntaxList = BuildSyntaxHelpers.GetTypeParameterSyntaxList(
                     typeParamsAndConstraints, itemAsT.Whitespace2Set, whitespaceLookup);
         if (typeParameterSyntaxList != null)
         {
            node = addTypeParameters(node, typeParameterSyntaxList);
            var clauses = BuildSyntaxHelpers.GetTypeParameterConstraintList(
                      typeParamsAndConstraints, itemAsT.Whitespace2Set, whitespaceLookup);
            if (clauses.Any())
            { node = addTypeParameterConstraints(node, clauses); }
         }
         return node;
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