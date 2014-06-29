namespace RoslynDom.Common
{
    public interface IMethod : IPropertyOrMethod, IHasTypeParameters
    {
        bool IsExtensionMethod { get; }
    }
}