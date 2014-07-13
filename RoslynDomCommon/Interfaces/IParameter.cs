namespace RoslynDom.Common
{
    public interface IParameter : IMisc, IHasAttributes, IDom<IParameter >
    {
        IReferencedType Type { get; }
        bool IsOut { get; }
        bool IsRef { get; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
        bool IsParamArray { get; }
        bool IsOptional { get; }
        int Ordinal { get; }
    }
}
