namespace RoslynDom.Common
{
    public interface INamespace : IStemMember, IStemContainer 
    {
        string OriginalName { get; }
    }
}