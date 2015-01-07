namespace RoslynDom.Common
{
   public interface IAccessor :
              IMisc,
              IStatementContainer,
              IHasAttributes,
              IHasAccessModifier,
              IDom<IAccessor>,
              IHasName
   {
      AccessorType AccessorType { get; }
   }
}