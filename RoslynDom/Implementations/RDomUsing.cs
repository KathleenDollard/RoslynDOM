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
    public class RDomUsingDirective : RDomSyntaxNodeBase<IUsing, UsingDirectiveSyntax, ISymbol>, IUsing
    {
        internal RDomUsingDirective(
            UsingDirectiveSyntax rawItem,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        { }

        internal RDomUsingDirective(RDomUsingDirective oldRDom)
             : base(oldRDom)
        { }

        public override string Name
        {
            get
            { return TypedSyntax.Name.NameFrom(); }
        }
    }
}
