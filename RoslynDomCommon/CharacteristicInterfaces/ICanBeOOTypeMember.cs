namespace RoslynDom.Common
{
    public interface IOOTypeMember : IDom
    {
        bool IsAbstract { get; set; }
        bool IsVirtual { get; set; }
        bool IsOverride { get; set; }
        bool IsSealed { get; set; }
    }
}
