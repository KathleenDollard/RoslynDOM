namespace RoslynDom.Common
{
    public interface IStemMember : IMember,  IDom, IHasName
    {
        StemMemberKind StemMemberKind { get; }
    }
}
