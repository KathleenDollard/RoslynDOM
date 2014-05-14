namespace RoslynDom.Common
{
    public interface INestableType : IRoslynDom, IStemMember
    {
        string OriginalName { get; }
    }
}