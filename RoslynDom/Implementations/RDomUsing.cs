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
    public class RDomUsingDirective : RDomSyntaxNodeBase<UsingDirectiveSyntax, ISymbol>, IUsing
    {
        internal RDomUsingDirective(UsingDirectiveSyntax rawItem) : base(rawItem) { }

        public override string Name
        {
            get
            { return TypedSyntax.Name.NameFrom(); }
        }

        public override string QualifiedName
        {
            get
            {
                throw new InvalidOperationException("You can't get qualified name for a using statement");
            }
        }
    }
}
