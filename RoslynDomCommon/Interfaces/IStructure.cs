namespace RoslynDom.Common
{
    public interface IStructure :
        IType<IStructure>, 
        INestedContainer, 
        ITypeMemberContainer, 
        IClassOrStructure,
        IHasTypeParameters, 
        IHasImplementedInterfaces
    {
    }
}