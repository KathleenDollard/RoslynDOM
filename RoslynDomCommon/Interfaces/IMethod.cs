namespace RoslynDom.Common
{
    public interface IMethod : IPropertyOrMethod<IMethod>, IHasTypeParameters
    {
        bool IsExtensionMethod { get; }
    }
}