namespace RoslynDom.Common
{
   public interface IHasInvocationFeatures
   {
      string MethodName { get; set; }

      RDomCollection<IReferencedType> TypeArguments { get; }

      RDomCollection<IArgument> Arguments { get; }
   }
}