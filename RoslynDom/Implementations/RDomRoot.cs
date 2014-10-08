using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomRoot : RDomBaseStemContainer<IRoot, ISymbol>, IRoot
   {
      // This takes a parent because in the future there will be a rootGroup concept for multiple files
      public RDomRoot(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomRoot(RDomRoot oldRDom)
         : base(oldRDom)
      { }

      [Required]
      public string Name { get; set; }
      public string FilePath { get; set; }

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
