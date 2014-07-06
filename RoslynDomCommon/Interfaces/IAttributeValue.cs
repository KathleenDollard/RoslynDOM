namespace RoslynDom.Common
{
    public interface IAttributeValue : IDom<IAttributeValue >
    {
        LiteralType  ValueType { get; }
        object Value { get; }
    }
}
