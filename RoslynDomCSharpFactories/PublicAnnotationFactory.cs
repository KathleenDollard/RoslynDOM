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
   public class PublicAnnotationFactory
       : RDomBaseItemFactory<IPublicAnnotation, SyntaxNode>, ITriviaFactory<IPublicAnnotation>
   {
      public PublicAnnotationFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return 0; } }

      public override Type[] SyntaxNodeTypes
      { get { return null; } }

      public override Type[] ExplicitNodeTypes
      { get { return new Type[] { typeof(IPublicAnnotation) }; } }

      protected override IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         IEnumerable<IMisc> list;
         var syntaxRoot = syntaxNode as CompilationUnitSyntax;
         if (syntaxRoot != null)
         {
            list = GetPublicAnnotations(syntaxRoot);
         }
         else
         { list = GetPublicAnnotations(syntaxNode); }
         return list;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         // This would make no sense because public annotations create trivia, not nodes. 
         return null;
      }

      public IPublicAnnotation CreateFrom(string possibleAnnotation, RDomCorporation corporation)
      {
         var str = GetMatch(possibleAnnotation);
         if (string.IsNullOrWhiteSpace(str)) return null;
         var target = str.SubstringBefore(":");
         var attribSyntax = GetAnnotationStringAsAttribute(str);
         // Reuse the evaluation work done in attribute to follow same rules
         var tempAttribute = corporation.Create(attribSyntax, null, null).FirstOrDefault() as IAttribute;
         var newPublicAnnotation = new RDomPublicAnnotation(tempAttribute.Name.ToString());
         newPublicAnnotation.Target = target;
         newPublicAnnotation.Whitespace2Set.AddRange(tempAttribute.Whitespace2Set);
         foreach (var attributeValue in tempAttribute.AttributeValues)
         {
            newPublicAnnotation.AddItem(attributeValue.Name ?? "", attributeValue.Value);
         }
         return newPublicAnnotation;
      }

      public IPublicAnnotation CreateFrom(SyntaxTrivia trivia, RDomCorporation corporation)
      {
         throw new NotImplementedException();
      }

      public IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IPublicAnnotation publicAnnotation)
      {
         throw new NotImplementedException();
      }

      #region Private methods to support public annotations

      private string GetMatch(string comment)
      {
         return comment.SubstringAfter("[[").SubstringBefore("]]").Trim();
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(System.String,System.String,Microsoft.CodeAnalysis.CSharp.CSharpParseOptions,System.Threading.CancellationToken)")]
      private static AttributeSyntax GetAnnotationStringAsAttribute(string str)
      {
         // Trick Roslyn into thinking it's an attribute
         str = "[" + str + "] public class {}";
         var tree = CSharpSyntaxTree.ParseText(str);
         var attrib = tree.GetRoot().DescendantNodes()
                     .Where(x => x.CSharpKind() == SyntaxKind.Attribute)
                     .FirstOrDefault();
         return attrib as AttributeSyntax;
      }

      private IEnumerable<RDomPublicAnnotation> GetPublicAnnotations(CompilationUnitSyntax syntaxRoot)
      {
         var ret = new List<RDomPublicAnnotation>();
         var nodes = syntaxRoot.ChildNodes();
         foreach (var node in nodes)
         {
            ret.AddRange(GetPublicAnnotationFromFirstToken(node, true));
         }
         return ret;
      }

      private IEnumerable<RDomPublicAnnotation> GetPublicAnnotations(SyntaxNode node)
      {
         return GetPublicAnnotationFromFirstToken(node, false);
      }

      private IEnumerable<RDomPublicAnnotation> GetPublicAnnotationFromFirstToken(
                 SyntaxNode node, bool isRoot)
      {
         var ret = new List<RDomPublicAnnotation>();
         var firstToken = node.GetFirstToken();
         if (firstToken != default(SyntaxToken))
         {
            ret.AddRange(GetPublicAnnotationFromToken(firstToken, isRoot));
         }
         return ret;
      }

      private IEnumerable<RDomPublicAnnotation> GetPublicAnnotationFromToken(
             SyntaxToken token, bool isRoot)
      {
         var ret = new List<RDomPublicAnnotation>();
         var trivias = token.LeadingTrivia
                           .Where(x => x.CSharpKind() == SyntaxKind.SingleLineCommentTrivia);
         foreach (var trivia in trivias)
         {
            var str = GetPublicAnnotationAsString(trivia);
            var strRoot = GetSpecialRootAnnotation(str);
            if (isRoot)
            { str = strRoot; }
            else
            { str = string.IsNullOrWhiteSpace(strRoot) ? str : ""; }
            if (!string.IsNullOrWhiteSpace(str))
            {
               var attribSyntax = GetAnnotationStringAsAttribute(str);
               // Reuse the evaluation work done in attribute to follow same rules
               var tempAttribute = OutputContext.Corporation.Create(attribSyntax, null, null).FirstOrDefault() as IAttribute;
               var newPublicAnnotation = new RDomPublicAnnotation(tempAttribute.Name.ToString());
               newPublicAnnotation.Whitespace2Set.AddRange(tempAttribute.Whitespace2Set);
               foreach (var attributeValue in tempAttribute.AttributeValues)
               {
                  newPublicAnnotation.AddItem(attributeValue.Name ?? "", attributeValue.Value);
               }
               ret.Add(newPublicAnnotation);
            }
         }
         return ret;
      }

      private static string GetPublicAnnotationAsString(SyntaxTrivia trivia)
      {
         var str = trivia.ToString().Trim();
         if (!str.StartsWith("//", StringComparison.Ordinal)) throw new InvalidOperationException("Unexpected comment format");
         str = str.SubstringAfter("//").SubstringAfter("[[").SubstringBefore("]]").Trim();
         return str;
      }

      private static string GetSpecialRootAnnotation(string str)
      {
         str = str.Trim();

         if (str.StartsWith("file:", StringComparison.Ordinal))
         { return str.SubstringAfter("file:"); }
         if (str.StartsWith("root:", StringComparison.Ordinal))
         { return str.SubstringAfter("root:"); }
         return null;
      }

 

      #endregion
   }
}
