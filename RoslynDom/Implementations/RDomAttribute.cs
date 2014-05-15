using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttribute : RDomSyntaxNodeBase<AttributeSyntax>, IAttribute
    {
        internal RDomAttribute(AttributeSyntax rawItem) : base(rawItem) { }

        public override string Name
        {
            get { return TypedRawItem.Name.ToString(); }
        }

        public override string QualifiedName
        {
            // TODO: Name of attribute is the same as the name of the type, so work this out
            get { return TypedRawItem.Name.QualifiedNameFrom(); }
        }

    }
}
