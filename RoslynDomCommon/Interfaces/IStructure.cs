namespace RoslynDom.Common
{
   public interface IStructure :
       IType<IStructure>,
       INestedContainer,
       IClassOrStructure,
       IHasTypeParameters,
       IHasImplementedInterfaces
   {
   }
}