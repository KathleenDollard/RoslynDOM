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
            get { return TypedRawItem.Name.QualifiedNameFrom(); }
        }

    }
}
