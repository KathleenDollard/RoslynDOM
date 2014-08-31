﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

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

        public bool IsConst { get; set; }

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
