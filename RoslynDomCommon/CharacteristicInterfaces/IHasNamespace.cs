namespace RoslynDom.Common
{
    public interface IHasNamespace : IHasName
    {
        string Namespace { get;  }
        string QualifiedName { get;  }
    }
}
