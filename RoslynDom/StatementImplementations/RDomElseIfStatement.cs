using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomElseIfStatement : RDomStatementBlockBase<IElseIfStatement>, IElseIfStatement
   {
      public RDomElseIfStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomElseIfStatement(RDomElseIfStatement oldRDom)
          : base(oldRDom)
      {
         Condition = oldRDom.Condition.Copy();
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = new List<IDom>();
            list.Add(Condition);
            list.AddRange(base.Children.ToList());
            return list;
         }
      }

      [Required]
      public IExpression Condition { get; set; }
   }
}
