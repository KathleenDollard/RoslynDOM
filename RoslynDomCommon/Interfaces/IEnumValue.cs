namespace RoslynDom.Common
{
    public interface IEnumValue : IDom<IEnumValue >, IHasAttributes , IHasName
    {
        IExpression Expression { get; set; }
    }
}