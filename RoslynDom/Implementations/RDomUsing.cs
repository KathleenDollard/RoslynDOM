using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomUsingDirective : RDomSyntaxNodeBase<UsingDirectiveSyntax>, IUsing
    {
        internal RDomUsingDirective(UsingDirectiveSyntax rawItem) : base(rawItem) { }

        public override string Name
        {
            get
            { return TypedRawItem.Name.NameFrom(); }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.Name.QualifiedNameFrom(); }
        }

        public override string BestInContextName
        {
            get { return TypedRawItem.Name.BestInContextNameFrom(); }
        }
    }
}
