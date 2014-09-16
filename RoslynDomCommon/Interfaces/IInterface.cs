namespace RoslynDom.Common
{
    public interface IInterface :   IType<IInterface>, 
                                    ITypeMemberContainer, 
                                    IHasTypeParameters ,
                                    IHasImplementedInterfaces 
    {
    }
}