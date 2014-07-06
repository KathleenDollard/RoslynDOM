namespace RoslynDom.Common
{
    public interface IStemMember : IMember,  IDom
    {
        StemMemberKind StemMemberKind { get; }
    }
}
