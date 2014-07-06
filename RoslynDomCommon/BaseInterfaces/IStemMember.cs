namespace RoslynDom.Common
{
    public interface IStemMember : IMember,  IDom
    {
        StemMemberType StemMemberType { get; }
    }
}
