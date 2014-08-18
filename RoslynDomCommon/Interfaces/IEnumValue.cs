namespace RoslynDom.Common
{
    public interface IEnumMember : IDom<IEnumMember >, IHasAttributes , IHasName, IMisc
    {
        IExpression Expression { get; set; }
    }
}