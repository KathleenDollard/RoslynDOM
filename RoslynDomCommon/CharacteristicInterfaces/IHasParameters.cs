namespace RoslynDom.Common
{
   public interface IHasParameters : IDom
   {
      RDomCollection<IParameter> Parameters { get; }
   }
}