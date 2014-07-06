namespace RoslynDom.Common
{
    public interface ITypeMember : IDom,IMember, IHasAttributes ,  IHasAccessModifier  
    {
        MemberKind MemberKind { get; }
    }
    public interface ITypeMember<T> : ITypeMember, IDom<T>
        where T : ITypeMember<T>
    {
    }
}
