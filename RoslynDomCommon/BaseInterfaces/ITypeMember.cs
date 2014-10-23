namespace RoslynDom.Common
{
 
    public interface ITypeMember :
            ITypeMemberAndDetail , 
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
