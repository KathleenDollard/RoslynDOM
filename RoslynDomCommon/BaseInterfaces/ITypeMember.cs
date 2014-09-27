namespace RoslynDom.Common
{
 
    public interface ITypeMember :
            ITypeMemberCommentWhite , 
            IMember, 
            IHasAttributes, 
            IHasAccessModifier, 
            IHasStructuredDocumentation, 
            IHasName
    {
    }
    public interface ITypeMember<T> : ITypeMember, IDom<T>
        where T : ITypeMember<T>
    {
    }
}
