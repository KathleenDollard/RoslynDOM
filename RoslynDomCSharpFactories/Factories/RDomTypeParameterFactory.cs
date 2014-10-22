using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomTypeParameterFactory
          : RDomMiscFactory<RDomTypeParameter, TypeParameterSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomTypeParameterFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      private WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterStart, SyntaxKind.LessThanToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterEnd, SyntaxKind.GreaterThanToken);
               _whitespaceLookup.Add(LanguageElement.ConstraintKeyword, SyntaxKind.WhereKeyword);
               _whitespaceLookup.Add(LanguageElement.ConstraintColon, SyntaxKind.ColonToken);
               _whitespaceLookup.Add(LanguageElement.ClassConstraint, SyntaxKind.ClassKeyword);
               _whitespaceLookup.Add(LanguageElement.ValueConstraint, SyntaxKind.StructKeyword);
               _whitespaceLookup.Add(LanguageElement.ConstructorConstraint, SyntaxKind.NewKeyword);
               // not attempting whitespace at constructor constraint right now because of low value and complexities with parens
               // ConstraintSeparator cannot be included here because it causes incorrect whitespace when storing ConstraintClause whitespace
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as TypeParameterSyntax;
         var newItem = new RDomTypeParameter(syntax, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         MemberWhitespace(newItem, syntax);

         var name = newItem.TypedSymbol.Name;
         newItem.Name = name;
         newItem.Variance = Mappings.VarianceFromVarianceKind(syntax.VarianceKeyword.CSharpKind());

         var typeParameterList = syntax.Parent.ChildNodes()
                     .OfType<TypeParameterSyntax>()
                     .Select(x => x.Identifier.ToString());
         newItem.Ordinal = typeParameterList.PreviousSiblings(name).Count();

         InitializeConstraints(syntax, newItem, model, name);
         return newItem;
      }

      private void MemberWhitespace(RDomTypeParameter newItem, TypeParameterSyntax syntax)
      {
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreListMemberWhitespace(syntax,
              SyntaxKind.CommaToken, LanguageElement.Identifier, newItem);
         //var whitespace2 = newItem.Whitespace2Set[LanguageElement.Identifier];
         //if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
         //{
         //   var prevNodeOrToken = syntax.Parent
         //                             .ChildNodesAndTokens()
         //                             .PreviousSiblings(syntax)
         //                             .LastOrDefault();
         //   var sepKind = SyntaxKind.CommaToken;
         //   if (prevNodeOrToken.CSharpKind() == sepKind)
         //   {
         //      var commaToken = prevNodeOrToken.AsToken();
         //      whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString();
         //   }
         //}
      }

      private void InitializeConstraints(TypeParameterSyntax syntax, RDomTypeParameter newItem, SemanticModel model, string name)
      {
         // parent is type parameter list. Parent parent contains constraints
         var syntaxParent = syntax.Parent.Parent;
         var constraintClauses = syntaxParent.ChildNodes()
                     .OfType<TypeParameterConstraintClauseSyntax>()
                     .ToList();
         var constraintClause = constraintClauses
                     .Where(x => x.Name.ToString() == name)
                     .FirstOrDefault();
         if (constraintClause != null)
         {
            // The constraint clause must be set first because the constructor constraint may change it. 
            CreateFromWorker.StoreWhitespace(newItem, constraintClause, LanguagePart.Constraint, WhitespaceLookup);
            // Class/Struct whitespace managed in above call, type explicitly handled below, constructor not handled by current design
            foreach (var constraint in constraintClause.Constraints)
            {
               var asClassStruct = constraint as ClassOrStructConstraintSyntax;
               if (asClassStruct != null)
               {
                  StoreClassOrStructureConstraint(newItem, asClassStruct.ClassOrStructKeyword.CSharpKind());
                  continue;
               }
               var asConstructor = constraint as ConstructorConstraintSyntax;
               if (asConstructor != null)
               {
                  StoreConstructorConstraint(asConstructor, newItem);
                  continue;
               }
               var asType = constraint as TypeConstraintSyntax;
               if (asType != null)
               {
                  StoreTypeConstraint(asType, newItem, model);
                  continue;
               }
            }

         }
      }

      private void StoreConstructorConstraint(ConstructorConstraintSyntax syntax,
          RDomTypeParameter newItem)
      {
         newItem.HasConstructorConstraint = true;
         CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntax, LanguagePart.Current, LanguageElement.ConstructorConstraint);
         CreateFromWorker.StoreListMemberWhitespace(syntax,
               SyntaxKind.CommaToken, LanguageElement.ConstructorConstraint, newItem);
         //var whitespace2 = newItem.Whitespace2Set[LanguageElement.ConstructorConstraint];
         //if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
         //{
         //   var prevNodeOrToken = syntax.Parent
         //                             .ChildNodesAndTokens()
         //                             .PreviousSiblings(syntax)
         //                             .LastOrDefault();
         //   var sepKind = SyntaxKind.CommaToken;
         //   if (prevNodeOrToken.CSharpKind() == sepKind)
         //   {
         //      var commaToken = prevNodeOrToken.AsToken();
         //      whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString();
         //   }
         //}
      }

      private void StoreConstraintWhitespace(TypeConstraintSyntax syntax, IReferencedType newItem)
      {
         CreateFromWorker.StoreListMemberWhitespace(syntax,
              SyntaxKind.CommaToken, LanguageElement.Identifier, newItem);
         //var whitespace2 = newItem.Whitespace2Set[LanguageElement.Identifier];
         //if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
         //{
         //   var prevNodeOrToken = syntax.Parent
         //                             .ChildNodesAndTokens()
         //                             .PreviousSiblings(syntax)
         //                             .LastOrDefault();
         //   var sepKind = SyntaxKind.CommaToken;
         //   if (prevNodeOrToken.CSharpKind() == sepKind)
         //   {
         //      var commaToken = prevNodeOrToken.AsToken();
         //      whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString();
         //   }
         //}

      }

      private void StoreTypeConstraint(TypeConstraintSyntax asType, RDomTypeParameter newItem, SemanticModel model)
      {
         var newConstraintType = Corporation
                 .CreateFrom<IMisc>(asType.Type, newItem, model)
                 .FirstOrDefault()
                 as IReferencedType;
         StoreConstraintWhitespace(asType, newConstraintType);
         if (newConstraintType != null)
         { newItem.ConstraintTypes.AddOrMove(newConstraintType); }
      }

      private static void StoreClassOrStructureConstraint(RDomTypeParameter newItem, SyntaxKind kind)
      {
         if (kind == SyntaxKind.ClassKeyword)
         { newItem.HasReferenceTypeConstraint = true; }
         if (kind == SyntaxKind.StructKeyword)
         { newItem.HasValueTypeConstraint = true; }
      }



      /// <summary>
      /// 
      /// </summary>
      /// <param name="item"></param>
      /// <returns></returns>
      /// <remarks>
      /// In C# syntax, type parameters and constraints are independent, so 
      /// for convenience (avoiding an extra pattern in the API) both are returned
      /// by this method. Use OfType<> to separately add them to the parent. 
      /// </remarks>
      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var list = new List<SyntaxNode>();
         var itemAsT = item as ITypeParameter;
         var node = SyntaxFactory.TypeParameter(itemAsT.Name);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         if (itemAsT.Variance != Variance.None)
         { node.WithVarianceKeyword(SyntaxFactory.Token(Mappings.VarianceKindFromVariance(itemAsT.Variance))); }
         list.Add(node);
         list.Add(GetConstraintClause(itemAsT.Name, itemAsT));

         return list.PrepareForBuildSyntaxOutput(item);
      }

      private SyntaxNode GetConstraintClause(string name, ITypeParameter itemAsT)
      {
         var list = new List<TypeParameterConstraintSyntax>();
         if (itemAsT.HasValueTypeConstraint)
         { list.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint)); }
         else if (itemAsT.HasReferenceTypeConstraint)
         { list.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint)); }

         foreach (var typeConstraint in itemAsT.ConstraintTypes)
         {
            var typeSyntax = (TypeSyntax)RDom.CSharp
                            .GetSyntaxNode(typeConstraint);
            var typeConstraintSyntax = SyntaxFactory.TypeConstraint(typeSyntax);
            list.Add(typeConstraintSyntax);
         }

         // New has to be last
         if (itemAsT.HasConstructorConstraint)
         {
            var constructorConstraint = SyntaxFactory.ConstructorConstraint();
            constructorConstraint = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(constructorConstraint, itemAsT.Whitespace2Set[LanguageElement.ConstructorConstraint]);
            list.Add(constructorConstraint);
         }

         var syntax = SyntaxFactory.TypeParameterConstraintClause(name)
                              .WithConstraints(SyntaxFactory.SeparatedList(list));
         syntax = BuildSyntaxHelpers.AttachWhitespace(syntax, itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Constraint);
         return syntax;
      }
   }

}
