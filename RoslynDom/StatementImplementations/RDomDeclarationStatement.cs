using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomDeclarationStatement : RDomBaseVariable, IDeclarationStatement
   {
      public RDomDeclarationStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomDeclarationStatement(RDomDeclarationStatement oldRDom)
           : base(oldRDom)
      {
         IsConst = oldRDom.IsConst;
      }

      private bool _isConst;
      public bool IsConst
      {
         get { return _isConst; }
         set { SetProperty(ref _isConst, value); }
      }

      IDeclarationStatement IDom<IDeclarationStatement>.Copy()
      {
         return (IDeclarationStatement)base.Copy();
      }

      public override string ToString()
      {
         return base.ToString() + " {" + Type.Name + "}";
      }
   }
}
