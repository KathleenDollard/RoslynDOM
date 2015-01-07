namespace RoslynDom.Common
{
   public interface IConversionOperator : ITypeMember<IConversionOperator>, ICanBeStatic, IStatementContainer, IHasParameters
   {
      // In C# it must be public and static, but other languages may not follow that rule
      bool IsImplicit { get; set; }

      IReferencedType Type { get; set; }
   }
}