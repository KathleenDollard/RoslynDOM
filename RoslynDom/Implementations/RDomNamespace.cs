using System;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

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

      public string Name { get; set; }
      public Guid Group { get; private set; }

      public StemMemberKind StemMemberKind
      { get { return StemMemberKind.Namespace; } }
   }
}
