using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomInterface : RDomBaseType<IInterface>, IInterface
   {
      public RDomInterface(string metadataName, AccessModifier declaredAccessModifier = AccessModifier.Private)
         : base(metadataName, declaredAccessModifier, MemberKind.Interface, StemMemberKind.Interface)
      { }

      public RDomInterface(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model, MemberKind.Interface, StemMemberKind.Interface)
      { }

      internal RDomInterface(RDomInterface oldRDom)
         : base(oldRDom)
      { }
   }
}