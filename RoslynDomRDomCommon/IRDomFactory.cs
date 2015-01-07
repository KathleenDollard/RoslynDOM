using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System;
using System.Collections.Generic;

namespace RoslynDom
{
   public interface IRDomFactory
   {
      RDomPriority Priority { get; }

      ///// <summary>
      /////
      ///// </summary>
      ///// <remarks>
      ///// Use Microsoft.CodeAnalysis.LanguageNames constants for names
      ///// </remarks>
      //string Language { get; }
      Type[] SupportedSyntaxNodeTypes { get; }

      Type[] SupportedDomTypes { get; }

      Type[] SpecialExplicitDomTypes { get; }

      bool CanCreate(SyntaxNode syntaxNode, IDom parent, SemanticModel model);

      bool CanGetSyntax(IDom item);

      bool CanCreateSpecialExplicit<TSpecial>(SyntaxNode syntaxNode, IDom parent, SemanticModel model);

      IEnumerable<IDom> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model, bool skipDetail);

      IEnumerable<SyntaxNode> BuildSyntax(IDom item);
   }
}