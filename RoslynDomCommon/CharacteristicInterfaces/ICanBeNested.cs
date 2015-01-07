namespace RoslynDom.Common
{
   public interface ICanBeNested : IDom
   {
      bool IsNested { get; }

      IType ContainingType { get; }

      string OuterName { get; }
   }
}