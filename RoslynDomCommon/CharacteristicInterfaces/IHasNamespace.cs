namespace RoslynDom.Common
{
    public interface IHasNamespace
    {
        string Namespace { get; }
        string QualifiedName { get; }
    }
}
