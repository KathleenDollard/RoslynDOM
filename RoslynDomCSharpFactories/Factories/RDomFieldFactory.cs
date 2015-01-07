using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomFieldTypeMemberFactory
         : RDomBaseSyntaxNodeFactory<RDomField, VariableDeclaratorSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomFieldTypeMemberFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.Static, SyntaxKind.StaticKeyword);
               _whitespaceLookup.Add(LanguageElement.NewSlot, SyntaxKind.NewKeyword);
               _whitespaceLookup.Add(LanguageElement.Readonly, SyntaxKind.ReadOnlyKeyword);
               _whitespaceLookup.Add(LanguageElement.Constant, SyntaxKind.ConstKeyword);
               _whitespaceLookup.Add(LanguageElement.Volatile, SyntaxKind.VolatileKeyword);
               _whitespaceLookup.Add(LanguageElement.EqualsAssignmentOperator, SyntaxKind.EqualsToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      public override Type[] SupportedSyntaxNodeTypes
      { get { return new Type[] { typeof(FieldDeclarationSyntax) }; } }

      protected override IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var list = new List<ITypeMember>();

         //var fieldPublicAnnotations = CreateFromWorker.GetPublicAnnotations(syntaxNode, parent, model);
         var rawField = syntaxNode as FieldDeclarationSyntax;
         var declarators = rawField.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
         foreach (var decl in declarators)
         {
            var newItem = new RDomField(decl, parent, model);
            list.Add(newItem);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, decl, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            if (decl.Initializer != null)
            {
               CreateFromWorker.StoreWhitespaceForToken(newItem, decl.Initializer.EqualsToken, LanguagePart.Current, LanguageElement.EqualsAssignmentOperator);
               CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, decl.Initializer, LanguagePart.Current, LanguageElement.Expression);
               newItem.Initializer = OutputContext.Corporation.CreateSpecial<IExpression>(decl.Initializer.Value, newItem, model).FirstOrDefault();
            }

            var returnType = OutputContext.Corporation
                             .Create(rawField.Declaration.Type, newItem, model)
                             .FirstOrDefault()
                             as IReferencedType;
            newItem.ReturnType = returnType;

            var fieldSymbol = newItem.Symbol as IFieldSymbol;
            newItem.IsStatic = fieldSymbol.IsStatic;
            newItem.IsVolatile = fieldSymbol.IsVolatile;
            newItem.IsReadOnly = fieldSymbol.IsReadOnly;
            newItem.IsConstant = fieldSymbol.HasConstantValue;
            // See note on IsNew on interface before changing
            newItem.IsNew = rawField.Modifiers.Any(x => x.CSharpKind() == SyntaxKind.NewKeyword);
            //newItem.PublicAnnotations.Add(fieldPublicAnnotations);
         }
         return list;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IField;
         var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
         var returnTypeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(itemAsT.ReturnType).First();
         var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
         if (itemAsT.IsReadOnly) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)); }
         if (itemAsT.IsConstant)
         {
            modifiers = modifiers.Remove(modifiers.Where(x => x.CSharpKind() == SyntaxKind.StaticKeyword).First());
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ConstKeyword));
         }
         if (itemAsT.IsVolatile) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.VolatileKeyword)); }
         var declaratorNode = SyntaxFactory.VariableDeclarator(nameSyntax);
         if (itemAsT.Initializer != null)
         {
            var expressionSyntax = (ExpressionSyntax)RDom.CSharp.GetSyntaxNode(itemAsT.Initializer);
            expressionSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(expressionSyntax, itemAsT.Whitespace2Set[LanguageElement.Expression]);
            var equalsToken = SyntaxFactory.Token(SyntaxKind.EqualsToken);
            equalsToken = BuildSyntaxHelpers.AttachWhitespaceToToken(equalsToken, itemAsT.Whitespace2Set[LanguageElement.EqualsAssignmentOperator]);
            var equalsValueClause = SyntaxFactory.EqualsValueClause(equalsToken, expressionSyntax);
            declaratorNode = declaratorNode.WithInitializer(equalsValueClause);
         }
         declaratorNode = BuildSyntaxHelpers.AttachWhitespace(declaratorNode, itemAsT.Whitespace2Set, WhitespaceLookup);

         var variableNode = SyntaxFactory.VariableDeclaration(returnTypeSyntax)
            .WithVariables(
                     SyntaxFactory.SingletonSeparatedList(declaratorNode));
         //variableNode = BuildSyntaxHelpers.AttachWhitespace(variableNode, itemAsField.Whitespace2Set, WhitespaceLookup);
         var node = SyntaxFactory.FieldDeclaration(variableNode)
            .WithModifiers(modifiers);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }
}