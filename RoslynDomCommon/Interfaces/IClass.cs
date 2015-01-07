namespace RoslynDom.Common
{
   public interface IClass :
       IType<IClass>,
       INestedContainer,
       IClassOrStructure,
       IHasTypeParameters,
       IHasImplementedInterfaces,
       ICanBeStatic
   {
      bool IsAbstract { get; set; }

      bool IsSealed { get; set; }

      bool IsPartial { get; set; }

      IReferencedType BaseType { get; set; }

      IDestructor Destructor { get; }
   }
}