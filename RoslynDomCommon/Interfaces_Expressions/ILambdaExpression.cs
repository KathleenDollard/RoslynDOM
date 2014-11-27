using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public interface  ILambdaExpression : IHasParameters, IExpression, IHasReturnType
   { }

   public interface ILambdaSingleExpression : ILambdaExpression
   {
      IExpression Expression { get; set; }
   }

   public interface ILambdaMultiLineExpression : ILambdaExpression, IStatementBlock
   { }
}
