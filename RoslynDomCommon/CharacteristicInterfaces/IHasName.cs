namespace RoslynDom.Common
{
   public interface IHasName : IDom
   {
      string Name { get; set; }
   }
}