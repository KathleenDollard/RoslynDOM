using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomUsingDirective : RDomBase<IUsing, UsingDirectiveSyntax, ISymbol>, IUsing
    {
        internal RDomUsingDirective(
            UsingDirectiveSyntax rawItem,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            Initialize();
        }

        internal RDomUsingDirective(RDomUsingDirective oldRDom)
             : base(oldRDom)
        { }

        protected override void Initialize()
        {
            base.Initialize();
           Name = TypedSyntax.Name.NameFrom(); 
        }
    }
}
