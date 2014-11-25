using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomRoot : RDomBaseStemContainer<IRoot, ISymbol>, IRoot
   {
      private Compilation _compilation;
      private ReferencedTypeCache _referencedTypeCache;

      //public RDomRoot(IDom parent, string name = null, string filePath = null)
      //: this(null, parent, null)
      //{
      //   _filePath = filePath;
      //}   
      
      // This takes a parent because in the future there will be a rootGroup concept for multiple files
      public RDomRoot(IFactoryAccess factoryAccess, SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
         var rootGroup = Parent as RDomRootGroup;
         if (rootGroup == null)
         {
            _compilation = model.Compilation;
            _referencedTypeCache = new ReferencedTypeCache(_compilation, factoryAccess);
         }
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomRoot(RDomRoot oldRDom)
         : base(oldRDom)
      {
         _name = oldRDom.Name;
         _filePath = oldRDom.FilePath;
      }

      internal IType FindByMetadataName(string metadataName)
      {
         var rootGroup = Parent as RDomRootGroup;
         if (rootGroup == null)
         { return _referencedTypeCache.FindByMetadataName(metadataName); }
         return rootGroup.FindByMetadataName(metadataName);
      }

      private string _name;
      [Required]
      public string Name
      {
         get {
            return string.IsNullOrWhiteSpace(_name) 
                     ? "<root>" 
                     :_name ; 
         }
         set { SetProperty(ref _name, value); }
      }

      private string _filePath;
      public string FilePath
      {
         get { return _filePath; }
         set { SetProperty(ref _filePath, value); }
      }

      public bool HasSyntaxErrors
      { get { return TypedSyntax.GetDiagnostics().Count() > 0; } }

      public IEnumerable<IClass> RootClasses
      {
         get
         {
            var rootclasses = from x in DescendantNamespaces
                              from y in x.Classes
                              select y;
            return Classes.Union(rootclasses);
         }
      }

      public IEnumerable<IEnum> RootEnums
      {
         get
         {
            var rootenums = from x in DescendantNamespaces
                            from y in x.Enums
                            select y;
            return Enums.Union(rootenums);
         }
      }

      public IEnumerable<IInterface> RootInterfaces
      {
         get
         {
            var rootinterfaces = from x in DescendantNamespaces
                                 from y in x.Interfaces
                                 select y;
            return Interfaces.Union(rootinterfaces);
         }
      }

      public IEnumerable<IStructure> RootStructures
      {
         get
         {
            var rootstructures = from x in DescendantNamespaces
                                 from y in x.Structures
                                 select y;
            return Structures.Union(rootstructures);
         }
      }
   }
}
