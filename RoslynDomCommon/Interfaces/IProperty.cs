namespace RoslynDom.Common
{
   public interface IProperty : IPropertyOrMethod<IProperty>
   {
      IReferencedType PropertyType { get; set; }

      bool CanGet { get; }

      bool CanSet { get; }

      IAccessor GetAccessor { get; set; }

      IAccessor SetAccessor { get; set; }
   }
}