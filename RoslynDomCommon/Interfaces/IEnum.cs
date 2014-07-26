namespace RoslynDom.Common
{
    public interface IEnum : IType<IEnum>
    {
        IReferencedType UnderlyingType { get; set; }
        RDomList<IEnumValue> Values { get; }
    }
}