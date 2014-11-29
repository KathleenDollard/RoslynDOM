using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomAttributeFactory : RDomBaseSyntaxNodeFactory<RDomAttribute, AttributeSyntax>
   {

      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomAttributeFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.AttributeSurroundStart, SyntaxKind.OpenBracketToken);
               _whitespaceLookup.Add(LanguageElement.AttributeSurroundEnd, SyntaxKind.CloseBracketToken);
               _whitespaceLookup.Add(LanguageElement.AttributeParameterStart, SyntaxKind.OpenParenToken);
               _whitespaceLookup.Add(LanguageElement.AttributeParameterEnd, SyntaxKind.CloseParenToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      public override RDomPriority Priority
      { get { return 0; } }

      public override Type[] SupportedSyntaxNodeTypes
      { get { return new Type[] { typeof(AttributeListSyntax), typeof(AttributeSyntax) }; } }

      protected override IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         return InternalCreateFrom(syntaxNode, parent, model);
      }

      private IEnumerable<IAttribute> InternalCreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntaxAsList = syntaxNode as AttributeListSyntax;

         if (syntaxAsList != null) { return CreateFromList(syntaxAsList, parent, model); }
         var attributeSyntax = syntaxNode as AttributeSyntax;
         if (attributeSyntax != null) { return new IAttribute[] { CreateFromItem(attributeSyntax, parent, model) }; }
         return null;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return BuildSyntax((IAttribute)item);
      }

      private IEnumerable<SyntaxNode> BuildSyntax(IAttribute item)
      {
         var itemAsT = item as IAttribute;
         var nameSyntax = SyntaxFactory.ParseName(itemAsT.Name);

         var node = SyntaxFactory.Attribute(nameSyntax);
         node = BuildSyntaxHelpers.AttachWhitespace(node, item.Whitespace2Set, WhitespaceLookup);
         var attributeArgList = itemAsT.AttributeValues
                         .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                         .OfType<AttributeArgumentSyntax>()
                         .ToList();

         if (attributeArgList.Any())
         {
            var argList = SyntaxFactory.AttributeArgumentList(
                            SyntaxFactory.SeparatedList(attributeArgList));
            argList = BuildSyntaxHelpers.AttachWhitespace(argList, item.Whitespace2Set, WhitespaceLookup);
            node = node.WithArgumentList(argList);
         }

         var nodeList = SyntaxFactory.AttributeList(
                             SyntaxFactory.SeparatedList(
                                 new AttributeSyntax[] {
                                        (AttributeSyntax)BuildSyntaxHelpers.PrepareForBuildItemSyntaxOutput(node, item,OutputContext )
                                 }));
         nodeList = BuildSyntaxHelpers.AttachWhitespace(nodeList, item.Whitespace2Set, WhitespaceLookup);

         return nodeList.PrepareForBuildSyntaxOutput(item, OutputContext);
      }

      #region Private methods to support adding attributes
      private IAttribute CreateFromItem(AttributeSyntax syntax, IDom parent, SemanticModel model)
      {
         var newItem = new RDomAttribute(syntax, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntax, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Parent, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.ArgumentList, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Name, LanguagePart.Current, WhitespaceLookup);

         newItem.Name = syntax.Name.ToString();

         if (syntax.ArgumentList != null)
         {
            newItem.AttributeValues.CreateAndAdd(syntax, x => x.ArgumentList.Arguments, x => OutputContext.Corporation.Create(x, newItem, model).Cast<IAttributeValue>());
            //var values = ListUtilities.MakeList(syntax, x => x.ArgumentList.Arguments, x => Corporation.Create(x, newItem, model))
            //      .OfType<IAttributeValue>();
            //  foreach (var value in values)
            //  { newItem.AddOrMoveAttributeValue(value); }
         }
         return newItem;
      }

      private IEnumerable<IAttribute> CreateFromList(AttributeListSyntax syntaxAsList, IDom parent, SemanticModel model)
      {
         var list = new List<IAttribute>();
         foreach (var attSyntax in syntaxAsList.Attributes)
         {
            var attr = CreateFromItem(attSyntax, parent, model);
            var rDomAttr = attr as IRoslynDom;
            list.Add(attr);
         }
         return list;
      }

   }
   #endregion
}
