namespace RoslynDom.Common
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
        "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification = "This refers to enums so seems correct")]
    public interface IEnum : IType<IEnum>, IContainer
    {
        IReferencedType UnderlyingType { get; set; }
        RDomCollection<IEnumMember> Members { get; }
    }
}