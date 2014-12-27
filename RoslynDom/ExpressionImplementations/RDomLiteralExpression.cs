using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace RoslynDom
{
   public class RDomLiteralExpression : RDomBaseExpression, ILiteralExpression
   {
      public RDomLiteralExpression(object literal)
      : base(null, null, null ,ExpressionType.Literal )
      {
         _literal = literal;
      }

      public RDomLiteralExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      {
         ExpressionType = ExpressionType.Literal;
      }

       [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomLiteralExpression(RDomLiteralExpression oldRDom)
          : base(oldRDom)
      {
         _literal = oldRDom.Literal;
      }

   
      private object _literal;
      [Required]
      public object Literal
      {
         get { return _literal; }
         set { SetProperty(ref _literal, value); }
      }
   }
}
