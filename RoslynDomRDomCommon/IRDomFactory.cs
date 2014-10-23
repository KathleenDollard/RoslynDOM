using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public interface IRDomFactory
   {
      ///// <summary>
      ///// 
      ///// </summary>
      ///// <remarks>
      ///// Use Microsoft.CodeAnalysis.LanguageNames constants for names
      ///// </remarks>
      //string Language { get; }
      Type[] SyntaxNodeTypes { get; }
      Type[] ExplicitNodeTypes { get; }
      Func<SyntaxNode, IDom, SemanticModel, bool> CanCreateDelegate { get; }
      Type DomType { get; }

      bool CanCreateFrom(SyntaxNode syntaxNode);
      bool CanBuildSyntax(IDom item);
      IEnumerable<IDom> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model, bool skipDetail);
      RDomPriority Priority { get; }
      IEnumerable<SyntaxNode> BuildSyntax(IDom item);
   }


   /// <summary>
   /// Priority for candidate selection. These are for clarity. Please add your
   /// own in the format "Normal + 1"
   /// </summary>
   public enum RDomPriority
   {
      None = 0,
      Fallback = 100,
      Normal = 200,
      Top = 300
   }
}
