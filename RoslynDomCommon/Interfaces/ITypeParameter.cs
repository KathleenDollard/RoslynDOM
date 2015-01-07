namespace RoslynDom.Common
{
   public interface ITypeParameter : IMisc, IDom<ITypeParameter>, IHasName
   {
      Variance Variance { get; set; }

      bool HasConstructorConstraint { get; set; }

      bool HasReferenceTypeConstraint { get; set; }

      bool HasValueTypeConstraint { get; set; }

      RDomCollection<IReferencedType> ConstraintTypes { get; }

      int Ordinal { get; set; }
   }
}