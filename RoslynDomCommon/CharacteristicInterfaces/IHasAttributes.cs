namespace RoslynDom.Common
{
   public interface IHasAttributes : IDom
   {
      AttributeCollection Attributes { get; }
   }
}