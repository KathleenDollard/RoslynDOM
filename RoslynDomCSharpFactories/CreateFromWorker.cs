using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class CreateFromWorker : ICSharpCreateFromWorker
   {
      private TriviaManager triviaManager = new TriviaManager();

      [ExcludeFromCodeCoverage]
      private static string nameof<T>(T value) { return ""; }

      private RDomCorporation corporation;
      public RDomCorporation Corporation
      {
         get { return corporation; }
         set
         {
            if (corporation != null) throw new InvalidOperationException("Can't reset corporation");
            corporation = value;
         }
      }

      public RDomPriority Priority
      { get { return RDomPriority.Normal; } }

      public void InitializeStatements(IStatementBlock itemAsStatement, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         if (syntaxNode == null) return;
         if (itemAsStatement == null) return;
         var blockSyntax = syntaxNode as BlockSyntax;
         if (blockSyntax != null)
         {
            itemAsStatement.StatementsAll.CreateAndAdd(blockSyntax.Statements, x => Corporation.Create(x, parent, model).Cast<IStatementAndDetail>());
            itemAsStatement.HasBlock = true;
            return;
         }
         var statementSyntax = syntaxNode as StatementSyntax;
         if (statementSyntax != null)
         {
            var statements = Corporation.Create(statementSyntax, parent, model).OfType<IStatementAndDetail>();
            if (statements.Count() > 1) throw new NotImplementedException();
            var statement = statements.First();
            var statementAsBlockStatement = statement as IBlockStatement;
            if (statementAsBlockStatement != null)
            {
               itemAsStatement.HasBlock = true;
               foreach (var st in statementAsBlockStatement.Statements)
               { itemAsStatement.StatementsAll.AddOrMove(st); }
            }
            else
            { itemAsStatement.StatementsAll.AddOrMove(statement); }
         }
      }

      public void StandardInitialize<T>(T newItem, SyntaxNode syntaxNode, IDom parent, SemanticModel model, OutputContext context)
              where T : class, IDom
      {
         //InitializePublicAnnotations(newItem, syntaxNode, parent, model);
         InitializeAttributes(newItem as IHasAttributes, syntaxNode, parent, model);
         InitializeAccessModifiers(newItem as IHasAccessModifier, syntaxNode, parent, model);
         InitializeOOTypeMember(newItem as IOOTypeMember, syntaxNode, parent, model);
         InitializeStatic(newItem as ICanBeStatic, syntaxNode, parent, model);
         InitializeStructuredDocumentation(newItem as IHasStructuredDocumentation, syntaxNode, parent, model, context);
         InitializeBaseList(newItem as IHasImplementedInterfaces, syntaxNode, parent, model);
         InitializeTypeParameters(newItem as IHasTypeParameters, syntaxNode, parent, model);
      }

      private void InitializeAccessModifiers(IHasAccessModifier itemHasAccessModifier, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         if (itemHasAccessModifier == null) { return; }
         var itemAsHasSymbol = itemHasAccessModifier as IRoslynHasSymbol;
         Guardian.Assert.IsNotNull(itemAsHasSymbol, nameof(itemAsHasSymbol));

         var accessibility = itemAsHasSymbol.Symbol.DeclaredAccessibility;
         itemHasAccessModifier.AccessModifier = Mappings.AccessModifierFromAccessibility(accessibility);
         var tokens = syntaxNode.ChildTokens();
         if (tokens.Any(x => x.CSharpKind() == SyntaxKind.PublicKeyword))
         { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Public; }
         else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.PrivateKeyword))
         { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Private; }
         else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.ProtectedKeyword) && tokens.Any(x => x.CSharpKind() == SyntaxKind.InternalKeyword))
         { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.ProtectedOrInternal; }
         else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.ProtectedKeyword))
         { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Protected; }
         else if (tokens.Any(x => x.CSharpKind() == SyntaxKind.InternalKeyword))
         { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.Internal; }
         else
         { itemHasAccessModifier.DeclaredAccessModifier = AccessModifier.None; }
      }

      //public void InitializePublicAnnotations(IDom item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      //{
      //   var publicAnnotations = GetPublicAnnotations(syntaxNode, item, model);
      //   item.PublicAnnotations.Add(publicAnnotations);
      //}

      private void InitializeAttributes(IHasAttributes itemAsHasAttributes,
                  SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         if (itemAsHasAttributes == null) { return; }
         var attributes = new List<IAttribute>();
         var attributeLists = syntaxNode.ChildNodes().OfType<AttributeListSyntax>();
         foreach (var attributeList in attributeLists)
         {
            // Flatten list
            // Force whitespace
            if (attributeList != null)
            {
               var attr = Corporation.Create(attributeList, itemAsHasAttributes, model).OfType<IAttribute>();
               attributes.AddRange(attr);
            }
         }
         itemAsHasAttributes.Attributes.AddOrMoveAttributeRange(attributes);
      }

      private void InitializeOOTypeMember(IOOTypeMember itemAsOO, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         if (itemAsOO == null) { return; }
         var itemAsDom = itemAsOO as IRoslynHasSymbol;
         //itemAsOO.IsAbstract = itemAsDom.Symbol.IsAbstract;
         itemAsOO.IsAbstract = syntaxNode.ChildTokens().Any(x => x.CSharpKind() == SyntaxKind.AbstractKeyword);
         itemAsOO.IsVirtual = itemAsDom.Symbol.IsVirtual;
         itemAsOO.IsOverride = itemAsDom.Symbol.IsOverride;
         itemAsOO.IsSealed = itemAsDom.Symbol.IsSealed;
         var itemAsCanBeNew = itemAsOO as ICanBeNew;
         if (itemAsCanBeNew != null)
         {

            // See note on IsNew on interface before changing
            itemAsCanBeNew.IsNew = syntaxNode.ChildTokens().Any(x => x.CSharpKind() == SyntaxKind.NewKeyword);
         }
      }

      private static WhitespaceKindLookup _whitespaceLookupForImplementedInterfaces;
      private static WhitespaceKindLookup whitespaceLookupForImplementedInterfaces
      {
         get
         {
            if (_whitespaceLookupForImplementedInterfaces == null)
            {
               _whitespaceLookupForImplementedInterfaces = new WhitespaceKindLookup();
               _whitespaceLookupForImplementedInterfaces.Add(LanguageElement.Separator, SyntaxKind.CommaToken);
               _whitespaceLookupForImplementedInterfaces.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookupForImplementedInterfaces.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookupForImplementedInterfaces;
         }
      }

      private void InitializeBaseList(IHasImplementedInterfaces itemAsT,
             SyntaxNode node, IDom parent, SemanticModel model)
      {
         if (itemAsT == null) return;

         var symbol = ((IRoslynHasSymbol)itemAsT).Symbol as INamedTypeSymbol;
         var interfaces = symbol.Interfaces;
         var baseType = symbol.BaseType;
         var baseList = node.ChildNodes().OfType<BaseListSyntax>().SingleOrDefault();
         if (baseList != null)
         {
            IEnumerable<BaseTypeSyntax> types = baseList.Types.ToList();
            StoreWhitespaceForToken(itemAsT, baseList.ColonToken, LanguagePart.Current, LanguageElement.BaseListPrefix);
            if (baseType != null)
            {
               var baseName = baseType.ToString();
               baseName = baseName.Contains(".")
                              ? baseName.SubstringAfterLast(".")
                              : baseName;
               if (node is ClassDeclarationSyntax && baseName == types.First().ToString())
               {
                  var itemAsClass = itemAsT as RDomClass;
                  var syntax = types.First();
                  if (itemAsClass == null) throw new InvalidOperationException();
                  var newBaseType = Corporation.Create(syntax, itemAsT, model).Single()
                                          as IReferencedType;
                  itemAsClass.BaseType = newBaseType;
                  //StoreWhitespace(newBaseType, syntax,
                  //              LanguagePart.Current, whitespaceLookupForImplementedInterfaces);
                  types = types.Skip(1);
               }
            }
            foreach (var implementedInterfaceSyntax in types)
            {
               var newInterface = Corporation.Create(implementedInterfaceSyntax, itemAsT, model).Single()
                               as IReferencedType;
               //StoreWhitespace(newInterface, implementedInterfaceSyntax,
               //              LanguagePart.Current, whitespaceLookupForImplementedInterfaces);

               var whitespace2 = newInterface.Whitespace2Set[LanguageElement.Identifier];
               if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
               {
                  var prevNodeOrToken = implementedInterfaceSyntax.Parent
                                            .ChildNodesAndTokens()
                                            .PreviousSiblings(implementedInterfaceSyntax)
                                            .LastOrDefault();
                  var sepKind = whitespaceLookupForImplementedInterfaces.Lookup(LanguageElement.Separator);
                  if (prevNodeOrToken.CSharpKind() == sepKind)
                  {
                     var commaToken = prevNodeOrToken.AsToken();
                     whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString();
                  }
               }

               itemAsT.ImplementedInterfaces.AddOrMove(newInterface);
            }
         }
      }

      private void InitializeTypeParameters(IHasTypeParameters itemAsT, SyntaxNode node, IDom parent, SemanticModel model)
      {
         if (itemAsT == null) return;

         //var symbol = ((IRoslynHasSymbol)itemAsT).Symbol as INamedTypeSymbol;
         //var interfaces = symbol.Interfaces;
         var typeParameterList = node.ChildNodes().OfType<TypeParameterListSyntax>().SingleOrDefault();
         if (typeParameterList == null) return;

         var typeParameters = typeParameterList.Parameters;
         foreach (var p in typeParameters)
         {
            var newBase = Corporation.Create(p, itemAsT, model).Single()
                            as ITypeParameter;
            itemAsT.TypeParameters.AddOrMove(newBase);
         }
      }

      private void InitializeStatic(ICanBeStatic item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         if (item == null) { return; }
         var itemAsDom = item as IRoslynHasSymbol;
         item.IsStatic = itemAsDom.Symbol.IsStatic;
      }

      private void InitializeStructuredDocumentation(IHasStructuredDocumentation item, SyntaxNode syntaxNode, IDom parent, SemanticModel model, OutputContext context)
      {
         if (item == null) return;
         var structuredDocumentation = GetStructuredDocumenation(syntaxNode, item, model, context).FirstOrDefault();
         if (structuredDocumentation != null)
         {
            item.StructuredDocumentation = structuredDocumentation;
            item.Description = structuredDocumentation.Description;
         }
      }

      public void LoadStemMembers(IStemContainer newItem,
                 IEnumerable<MemberDeclarationSyntax> memberSyntaxes,
                 IEnumerable<UsingDirectiveSyntax> usingSyntaxes,
                 SemanticModel model)
      {
         newItem.StemMembersAll.CreateAndAdd(usingSyntaxes, x => Corporation.Create(x, newItem, model).Cast<IStemMemberAndDetail>());
         newItem.StemMembersAll.CreateAndAdd(memberSyntaxes, x => Corporation.Create(x, newItem, model).Cast<IStemMemberAndDetail>());
      }

      public IEnumerable<IDetail> GetDetail<T, TSyntax>(TSyntax syntaxNode, SyntaxTriviaList triviaList, T parent, SemanticModel model, OutputContext context)
         where T : class, IDom
         where TSyntax : SyntaxNode
      {
         var ret = new List<IDetail>();
         var lastWasComment = false;
         var precedingTrivia = new List<SyntaxTrivia>();
         foreach (var trivia in triviaList)
         {
            // This is ugly, but we assume comments stand on their own lines. 
            var skip = (lastWasComment && trivia.CSharpKind() == SyntaxKind.EndOfLineTrivia);
            lastWasComment = false;
            if (!skip)
            {
               switch (trivia.CSharpKind())
               {
                  case SyntaxKind.EndOfLineTrivia:
                     // TODO: Consider whether leading WS on a vert whitespace matters
                     ret.Add(new RDomVerticalWhitespace(trivia, 1, false));
                     break;
                  case SyntaxKind.SingleLineCommentTrivia:
                  case SyntaxKind.MultiLineCommentTrivia:
                     ret.Add(MakeComment(syntaxNode, precedingTrivia, trivia, parent, context));
                     lastWasComment = true;
                     break;
                  case SyntaxKind.RegionDirectiveTrivia:
                  case SyntaxKind.EndRegionDirectiveTrivia:
                     ret.Add(context.Corporation.GetTriviaFactory<IDetail>().CreateFrom(trivia, parent, context) as IDetail);
                     break;
               }
            }
            precedingTrivia.Add(trivia);
         }
         return ret;
      }

      //public IEnumerable<IDetail> GetDetail<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model, OutputContext context)
      //    where T : class, IDom
      //    where TSyntax : SyntaxNode
      //{
      //   //return Corporation.Create<IDetail>(syntaxNode, newItem, model);

      //   var ret = new List<IDetail>();
      //   // The parent of the syntax is the next item. The parent of the region is the thing it's attached to
      //   var parent = newItem.Parent;
      //   if (!syntaxNode.HasLeadingTrivia) return ret;
      //   var triviaList = syntaxNode.GetLeadingTrivia();
      //   var lastWasComment = false;
      //   var precedingTrivia = new List<SyntaxTrivia>();
      //   foreach (var trivia in triviaList)
      //   {
      //      // This is ugly, but we assume comments stand on their own lines. 
      //      var skip = (lastWasComment && trivia.CSharpKind() == SyntaxKind.EndOfLineTrivia);
      //      lastWasComment = false;
      //      if (!skip)
      //      {
      //         switch (trivia.CSharpKind())
      //         {
      //            case SyntaxKind.EndOfLineTrivia:
      //               // TODO: Consider whether leading WS on a vert whitespace matters
      //               ret.Add(new RDomVerticalWhitespace(trivia, 1, false));
      //               break;
      //            case SyntaxKind.SingleLineCommentTrivia:
      //            case SyntaxKind.MultiLineCommentTrivia:
      //               ret.Add(MakeComment(syntaxNode, precedingTrivia, trivia, parent, context));
      //               lastWasComment = true;
      //               break;
      //            case SyntaxKind.RegionDirectiveTrivia:
      //            case SyntaxKind.EndRegionDirectiveTrivia:
      //               ret.Add(context.Corporation.GetTriviaFactory<IDetail>().CreateFrom(trivia, parent, context) as IDetail);
      //               break;
      //         }
      //      }
      //      precedingTrivia.Add(trivia);
      //   }
      //   return ret;
      //}

      private IDetail MakeComment(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia, IDom parent, OutputContext context)
      {
         var publicAnnotation = context.Corporation.GetTriviaFactory<IPublicAnnotation>().CreateFrom(trivia, parent, context) as IPublicAnnotation;
         if (publicAnnotation != null) return publicAnnotation;
         var newComment = context.Corporation.GetTriviaFactory<IDetail>().CreateFrom(trivia, parent, context) as IComment;
         return newComment;
      }

      //private IDetailBlockEnd MakeRegion(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia, IDom parent, OutputContext context)
      //{
      //   if (!trivia.HasStructure) return null;
      //   var structure = trivia.GetStructure();
      //   var regionSyntax = structure as RegionDirectiveTriviaSyntax;
      //   var text = regionSyntax.EndOfDirectiveToken.ToFullString().Replace("\r\n", "");
      //   var newRegion = new RDomDetailBlockStart(regionSyntax, parent, null, text);
      //   return newRegion;
      //}

      //private IBlockEndDetail MakeEndRegion(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia, IDom parent, OutputContext context)
      //{
      //   if (!trivia.HasStructure) return null;
      //   var structure = trivia.GetStructure();
      //   var regionSyntax = structure as EndRegionDirectiveTriviaSyntax;
      //   var startDirectives = regionSyntax
      //                           .GetRelatedDirectives()
      //                           .Where(x => x is RegionDirectiveTriviaSyntax);
      //   if (startDirectives.Count() != 1) { throw new NotImplementedException(); }
      //   var startSyntax = startDirectives.Single();
      //   var newRegion = new RDomRegionEnd(regionSyntax, parent, null, startSyntax);
      //   return newRegion;
      //}

      public Tuple<string, string, string> ExtractComment(string text)
      {

         if (text.StartsWith("//")) { text = text.Substring(2); }
         if (text.StartsWith("/*"))
         {
            text = text.Substring(2);
            if (text.EndsWith("*/"))
            { text = text.Substring(0, text.Length - 2); }
         }
         // TODO: Ensure you test with whitespace only comment of both types
         var trailing = text.SubstringAfter(text.TrimEnd());
         var leading = text.SubstringBefore(text.TrimStart());
         return Tuple.Create(leading, text.Trim(), trailing);
      }

      // public IEnumerable<IPublicAnnotation> GetPublicAnnotations<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model, OutputContext context)
      //   where T : class, IDom
      //   where TSyntax : SyntaxNode
      //{
      //   return Corporation.Create<IPublicAnnotation>(syntaxNode, newItem, model);
      //}

      public IEnumerable<IStructuredDocumentation> GetStructuredDocumenation<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model, OutputContext context)
          where T : class, IDom
          where TSyntax : SyntaxNode
      {
         return Corporation.Create<IStructuredDocumentation>(syntaxNode, newItem, model);
      }

      // TODO: Refactor - D'oh
      public Tuple<object, string, LiteralKind> GetArgumentValue(IDom newItem, SemanticModel model, ExpressionSyntax expr)
      {
         // TODO: Refactor this method
         var literalKind = LiteralKind.Unknown;
         object value = null;
         string constantIdentifier = null;

         if (!TryLiteralExpression(expr as LiteralExpressionSyntax, newItem, model, ref value, ref literalKind, ref constantIdentifier))
            if (!TryTyepofExpression(expr as TypeOfExpressionSyntax, newItem, model, ref value, ref literalKind, ref constantIdentifier))
               if (!TryDefaultExpression(expr as DefaultExpressionSyntax, newItem, model, ref value, ref literalKind, ref constantIdentifier))
                  if (!TryMemberExpression(expr as MemberAccessExpressionSyntax, newItem, model, ref value, ref literalKind, ref constantIdentifier))
                     //if (!TryIdentifierExpression(expr as IdentifierNameSyntax, newItem, model, ref value, ref literalKind, ref constantIdentifier))
                     { }
         return Tuple.Create(value, constantIdentifier, literalKind);
      }

      private bool TryIdentifierExpression(MemberAccessExpressionSyntax identifierExpression,
                    IDom newItem,
                    SemanticModel model,
                    ref object value,
                    ref LiteralKind literalKind,
                    ref string constantIdentifier)
      {
         if (identifierExpression == null)
         { return false; }
         literalKind = LiteralKind.Identifier;
         constantIdentifier = identifierExpression.ToFullString();
         value = identifierExpression.ToFullString();
         return true;
      }

      private bool TryMemberExpression(MemberAccessExpressionSyntax memberAccessExpression,
                     IDom newItem,
                     SemanticModel model,
                     ref object value,
                     ref LiteralKind literalKind,
                     ref string constantIdentifier)
      {
         if (memberAccessExpression == null)
         { return false; }
         literalKind = LiteralKind.MemberAccess;
         // If this is legal code, this is a constant or an enum
         var constant = model.GetConstantValue(memberAccessExpression);
         if (constant.HasValue)
         {
            constantIdentifier = memberAccessExpression.ToString();
            value = constant.Value;
         }
         else
         {
            constantIdentifier = memberAccessExpression.ToFullString();
            value = memberAccessExpression.ToFullString();
         }
         return true;
      }

      private bool TryDefaultExpression(DefaultExpressionSyntax defaultExpression,
                     IDom newItem,
                     SemanticModel model,
                     ref object value,
                     ref LiteralKind literalKind,
                     ref string constantIdentifier)
      {
         if (defaultExpression == null)
         { return false; }
         literalKind = LiteralKind.Default;
         value = Corporation
              .Create(defaultExpression.Type, newItem, model)
              .FirstOrDefault()
              as IReferencedType;
         return true;
      }

      private bool TryTyepofExpression(TypeOfExpressionSyntax typeOfExpression,
                     IDom newItem,
                     SemanticModel model,
                     ref object value,
                     ref LiteralKind literalKind,
                     ref string constantIdentifier)
      {
         if (typeOfExpression == null)
         { return false; }
         literalKind = LiteralKind.Type;
         value = Corporation
              .Create(typeOfExpression.Type, newItem, model)
              .FirstOrDefault()
              as IReferencedType;
         return true;
      }

      private bool TryLiteralExpression(LiteralExpressionSyntax literalExpression,
                     IDom newItem,
                     SemanticModel model,
                     ref object value,
                     ref LiteralKind literalKind,
                     ref string constantIdentifier)
      {
         if (literalExpression == null)
         { return false; }
         literalKind = Mappings.LiteralKindFromSyntaxKind(literalExpression.Token.CSharpKind());
         value = literalExpression.Token.Value;
         return true;
      }

      public IEnumerable<IDom> CreateInvalidMembers(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var ret = new RDomInvalidMember(syntaxNode, parent, model);
         Guardian.Assert.IsNotNull(ret, nameof(ret));
         return new List<IDom>() { ret };
      }

      public void StoreWhitespace(IDom newItem, SyntaxNode syntaxNode, LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup)
      { triviaManager.StoreWhitespace(newItem, syntaxNode, languagePart, whitespaceLookup); }

      public void StoreWhitespaceForToken(IDom newItem, SyntaxToken token,
              LanguagePart languagePart, LanguageElement languageElement)
      { triviaManager.StoreWhitespaceForToken(newItem, token, languagePart, languageElement); }

      public void StoreWhitespaceForFirstAndLastToken(IDom newItem, SyntaxNode node,
              LanguagePart languagePart,
              LanguageElement languageElement)
      { triviaManager.StoreWhitespaceForFirstAndLastToken(newItem, node, languagePart, languageElement); }

      public void StoreListMemberWhitespace(SyntaxNode syntax,
                    SyntaxKind syntaxKind,
                    LanguageElement elementType,
                    IDom newItem)
      {
         triviaManager.StoreListMemberWhitespace(syntax, syntaxKind, elementType, newItem);
      }
   }
}
