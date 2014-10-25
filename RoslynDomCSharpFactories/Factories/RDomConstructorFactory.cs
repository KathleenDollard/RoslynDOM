using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomConstructorTypeMemberFactory
         : RDomBaseItemFactory<RDomConstructor, ConstructorDeclarationSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomConstructorTypeMemberFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.ParameterStartDelimiter, SyntaxKind.OpenParenToken);
               _whitespaceLookup.Add(LanguageElement.ParameterEndDelimiter, SyntaxKind.CloseParenToken);
               _whitespaceLookup.Add(LanguageElement.ConstructorInitializerPrefix, SyntaxKind.ColonToken);
               _whitespaceLookup.Add(LanguageElement.ConstructorBaseInitializer, SyntaxKind.BaseKeyword);
               _whitespaceLookup.Add(LanguageElement.ConstructorThisInitializer, SyntaxKind.ThisKeyword);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ConstructorDeclarationSyntax;
         var newItem = new RDomConstructor(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Initializer, LanguagePart.Initializer, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Body, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.ParameterList, LanguagePart.Current, WhitespaceLookup);

         newItem.Name = newItem.TypedSymbol.Name;

         //newItem.AccessModifier = RoslynUtilities.GetAccessibilityFromSymbol(newItem.Symbol);
         //newItem.IsStatic = newItem.Symbol.IsStatic;

         newItem.Parameters.CreateAndAdd(syntax, x => x.ParameterList.Parameters, x => OutputContext.Corporation.Create(x, newItem, model).Cast<IParameter>());
         //var parameters = ListUtilities.MakeList(syntax, x => x.ParameterList.Parameters, x => Corporation.Create(x, newItem, model))
         //                    .OfType<IParameter>();
         //newItem.Parameters.AddOrMoveRange(parameters);


         if (syntax.Initializer == null)
         { newItem.ConstructorInitializerType = ConstructorInitializerType.None; }
         else
         {
            var initializerSyntax = syntax.Initializer;
            if (initializerSyntax.ThisOrBaseKeyword.ToString() == "this")
            { newItem.ConstructorInitializerType = ConstructorInitializerType.This; }
            else
            { newItem.ConstructorInitializerType = ConstructorInitializerType.Base; }
            CreateFromWorker.StoreWhitespace(newItem, initializerSyntax.ArgumentList,
                        LanguagePart.Initializer, WhitespaceLookup);
            foreach (var arg in initializerSyntax.ArgumentList.Arguments)
            {
               var newArg = new RDomArgument(arg, newItem, model);
               newArg.ValueExpression = OutputContext.Corporation.Create<IExpression>(arg.Expression, newItem, model).FirstOrDefault();
               CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newArg, arg, LanguagePart.Current, LanguageElement.ConstructorInitializerArgument);
               CreateFromWorker.StoreListMemberWhitespace(arg,
                    SyntaxKind.CommaToken, LanguageElement.ConstructorInitializerArgument, newArg);
               newItem.InitializationArguments.AddOrMove(newArg);
            }
         }

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IConstructor;
         var parent = item.Parent as IClass;
         SyntaxToken nameSyntax;
         if (parent == null)
         { nameSyntax = SyntaxFactory.Identifier("unknown_name"); }
         else
         { nameSyntax = SyntaxFactory.Identifier(parent.Name); }

         var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
         var node = SyntaxFactory.ConstructorDeclaration(nameSyntax)
                         .WithModifiers(modifiers);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var parameterList = itemAsT.Parameters
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .OfType<ParameterSyntax>()
                     .ToList();

         var parameterListSyntax = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList));
         parameterListSyntax = BuildSyntaxHelpers.AttachWhitespace(parameterListSyntax, itemAsT.Whitespace2Set, WhitespaceLookup);
         node = node.WithParameterList(parameterListSyntax);

         //node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

         node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup));

         var initializer = BuildInitializer(itemAsT);
         if (initializer != null)
         {
            initializer = BuildSyntaxHelpers.AttachWhitespace(initializer,
                itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Initializer);
            node = node.WithInitializer(initializer);
         }

         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }

      private ConstructorInitializerSyntax BuildInitializer(IConstructor itemAsT)
      {
         ConstructorInitializerSyntax node;

         switch (itemAsT.ConstructorInitializerType)
         {
            case ConstructorInitializerType.None:
               { return null; }
            case ConstructorInitializerType.Base:
               {
                  node = SyntaxFactory.ConstructorInitializer(
                              SyntaxKind.BaseConstructorInitializer);
                  break;
               }
            case ConstructorInitializerType.This:
               {
                  node = SyntaxFactory.ConstructorInitializer(
                              SyntaxKind.ThisConstructorInitializer);
                  break;
               }
            default:
               throw new InvalidOperationException();
         }
         var argList = itemAsT.InitializationArguments
                        .Select(x => GetArgumentSyntax(x));
         var argListSyntax = SyntaxFactory.ArgumentList(
                         SyntaxFactory.SeparatedList(argList));
         argListSyntax = BuildSyntaxHelpers.AttachWhitespace(argListSyntax,
                 itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Initializer);
         node = node.WithArgumentList(argListSyntax);
         return node;
      }

      private ArgumentSyntax GetArgumentSyntax(IArgument arg)
      {
         var expressionSyntax = (ExpressionSyntax)RDom.CSharp.GetSyntaxNode(arg.ValueExpression);
         expressionSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(expressionSyntax, arg.Whitespace2Set[LanguageElement.Expression]);
         var argSyntax = SyntaxFactory.Argument(expressionSyntax);
         argSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirst(argSyntax, arg.Whitespace2Set[LanguageElement.ConstructorInitializerArgument ]);
         return argSyntax;
         //argSyntax = BuildSyntaxHelpers.AttachWhitespace(argSyntax, arg.Whitespace2Set, WhitespaceLookup);
         //argSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirst(argSyntax, arg.Whitespace2Set[LanguageElement.ConstructorInitializerArgument]);
         //argSyntax = BuildSyntaxHelpers.AttachWhitespaceToLast(argSyntax, arg.Whitespace2Set[LanguageElement.ParameterLastToken]);
         //return (ArgumentSyntax)argSyntax.PrepareForBuildSyntaxOutput(arg).First();
      }
   }
}
