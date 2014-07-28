namespace RoslynDom.Common
{
    public interface IHasCondition : IDom
    {
        IExpression Condition { get; set; }
    }
}
