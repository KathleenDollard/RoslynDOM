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
         Name = name;
         Group = group;
      }

      public RDomNamespace(SyntaxNode rawItem, IDom parent, SemanticModel model, string name, Guid group)
           : base(rawItem, parent, model)
      {
         Name = name;
         Group = group;
      }

      internal RDomNamespace(RDomNamespace oldRDom)
          : base(oldRDom)
      {
         Group = oldRDom.Group;
      }

      private string _name ;
      [Required]
      public string Name { get {return _name; }
set {SetProperty(ref _name, value); }}
      private Guid _group ;
      public Guid Group { get {return _group; }
private set {SetProperty(ref _group, value); }}

      public StemMemberKind StemMemberKind
      { get { return StemMemberKind.Namespace; } }
   }
}
