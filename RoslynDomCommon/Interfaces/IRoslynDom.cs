namespace RoslynDom.Common
{
    public interface IRoslynDom
    {
        object RawItem { get; }
        string Name { get; }
        string QualifiedName { get; }
    }
}