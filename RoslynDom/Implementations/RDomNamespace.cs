using System;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomNamespace : RDomBaseStemContainer<INamespace, INamespaceSymbol>, INamespace
   {
      public RDomNamespace(string name, Guid group = default(Guid))
        : base(null, null, null)
      {
         _name = name;
         _group = group;
      }

      public RDomNamespace(SyntaxNode rawItem, IDom parent, SemanticModel model, string name, Guid group)
           : base(rawItem, parent, model)
      {
         _name = name;
         _group = group;
      }

      internal RDomNamespace(RDomNamespace oldRDom)
          : base(oldRDom)
      {
         _name = oldRDom.Name;
         _group = oldRDom.GroupGuid;
      }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }
      private Guid _group;
      public Guid GroupGuid
      {
         get { return _group; }
         private set { SetProperty(ref _group, value); }
      }

      public StemMemberKind StemMemberKind
      { get { return StemMemberKind.Namespace; } }
   }
}
