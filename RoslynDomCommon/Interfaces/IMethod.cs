namespace RoslynDom.Common
{
    public interface IMethod : IPropertyOrMethod<IMethod>, IHasTypeParameters, IStatementContainer 
    {
        bool IsExtensionMethod { get; }
    }
}