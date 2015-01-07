namespace RoslynDom.Common
{
   public interface ICanBeStatic : IDom
   {
      bool IsStatic { get; set; }
   }
}