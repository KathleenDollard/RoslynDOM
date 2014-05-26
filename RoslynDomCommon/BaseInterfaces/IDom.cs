namespace RoslynDom.Common
{
    public interface IDom
    {
        object RawSyntax { get; }
        string Name { get; }
        string QualifiedName { get; }
        string OuterName { get; }

        string Namespace { get; }
    }
}