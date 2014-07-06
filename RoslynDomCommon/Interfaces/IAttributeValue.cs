namespace RoslynDom.Common
{
    public interface IAttributeValue : IDom<IAttributeValue >
    {
        LiteralKind  ValueType { get; }
        object Value { get; }
    }
}
