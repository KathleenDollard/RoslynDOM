namespace RoslynDom.Common
{
   public interface IParameter : IMisc, IHasAttributes, IDom<IParameter>, IHasName
   {
      IReferencedType Type { get; set; }
      bool IsOut { get; set; }
      bool IsRef { get; set; }
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
      bool IsParamArray { get; set; }
      bool IsOptional { get; set; }
      object DefaultValue { get; set; }
      string DefaultConstantIdentifier { get; set; }
      LiteralKind DefaultValueType { get; set; }
      int Ordinal { get; set; }
   }
}
