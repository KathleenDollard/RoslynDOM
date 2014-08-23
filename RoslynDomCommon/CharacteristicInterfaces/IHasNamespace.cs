namespace RoslynDom.Common
{
    public interface IHasNamespace : IHasName,  IDom
    {
        string Namespace { get;  }
        string QualifiedName { get; }
        string ContainingTypeName { get; }
    }
}
