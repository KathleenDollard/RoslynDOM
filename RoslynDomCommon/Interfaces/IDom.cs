namespace RoslynDom.Common
{
    public interface IDom
    {
        object RawItem { get; }
        string Name { get; }
        string QualifiedName { get; }
        string OuterName { get; }
    }
}