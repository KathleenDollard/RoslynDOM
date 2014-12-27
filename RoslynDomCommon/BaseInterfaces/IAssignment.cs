using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public interface IAssignment
   {
      IExpression Expression { get; set; }
      IExpression Left { get; set; }
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
         "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Operator",
        Justification = "Because this represents an operator, it's seems an appropriate name")]
      AssignmentOperator Operator { get; set; }
   }
}
