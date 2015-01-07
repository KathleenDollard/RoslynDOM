namespace RoslynDom.Common
{
   public interface IHasTypeParameters : IDom
   {
      RDomCollection<ITypeParameter> TypeParameters { get; }
   }
}