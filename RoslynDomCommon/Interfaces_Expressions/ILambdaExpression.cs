namespace RoslynDom.Common
{
   public interface ILambdaExpression : IHasParameters, IExpression, IHasReturnType
   { }

   public interface ILambdaSingleExpression : ILambdaExpression
   {
      IExpression Expression { get; set; }
   }

   public interface ILambdaMultiLineExpression : ILambdaExpression, IStatementBlock
   { }
}