namespace RoslynDom.Common
{
    public interface IStemMemberCommentWhite : IDom
    {
        StemMemberKind StemMemberKind { get; }
    }
}
