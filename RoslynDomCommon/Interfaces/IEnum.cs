namespace RoslynDom.Common
{
    public interface IEnum : IType<IEnum>
    {
        IReferencedType UnderlyingType { get; set; }
        RDomList<IEnumMember> Members { get; }
    }
}