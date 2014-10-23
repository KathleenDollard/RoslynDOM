namespace RoslynDom.Common
{
    public interface ITypeMemberAndDetail : IDom
    {
        MemberKind MemberKind { get; }
    }


}
