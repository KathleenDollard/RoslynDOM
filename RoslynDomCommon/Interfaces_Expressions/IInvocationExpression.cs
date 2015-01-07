namespace RoslynDom.Common
{
   public interface IInvocationExpression : IHasInvocationFeatures, IExpression
   {
      RDomCollection<IArgument> Arguments { get; }
   }
}