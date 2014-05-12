using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{
    public class KAttribute : KSyntaxNodeBase<AttributeSyntax>, IAttribute
    {
        internal KAttribute(AttributeSyntax rawItem) : base(rawItem) { }

        public override string Name
        {
            get { return TypedRawItem.Name.ToString(); }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.Name.QNameFrom(); }
        }

        public override string BestInContextName
        {
            get { return TypedRawItem.Name.BestInContextNameFrom(); }
        }
    }
}
