namespace RoslynDom.Common
{
    public interface IEnum : IType
    {
        IReferencedType UnderlyingType { get; }
    }
}