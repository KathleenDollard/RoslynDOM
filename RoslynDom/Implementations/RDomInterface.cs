using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomInterface : RDomBaseType<IInterface>, IInterface
   {
      public RDomInterface(string name, AccessModifier accessModifier = AccessModifier.Private)
      : base(name, accessModifier, MemberKind.Interface, StemMemberKind.Interface)
      { }

      public RDomInterface(SyntaxNode rawItem, IDom parent, SemanticModel model)
     : base(rawItem, parent, model, MemberKind.Interface, StemMemberKind.Interface)
      { }

      internal RDomInterface(RDomInterface oldRDom)
           : base(oldRDom)
      { }
   }
}
