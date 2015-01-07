namespace RoslynDom.Common
{
   public interface IReferencedType : IMisc, IDom<IReferencedType>, IHasNamespace, IHasName
   {
      string MetadataName { get; set; }

      bool DisplayAlias { get; set; }

      bool IsArray { get; set; }

      RDomCollection<IReferencedType> TypeArguments { get; }

      IType Type { get; }
   }
}