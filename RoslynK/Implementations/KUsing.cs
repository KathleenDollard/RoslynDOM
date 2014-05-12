using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{
    public class KUsingDirective : KSyntaxNodeBase<UsingDirectiveSyntax>, IUsing
    {
        internal KUsingDirective(UsingDirectiveSyntax rawItem) : base(rawItem) { }

        public override string Name
        {
            get
            { return TypedRawItem.Name.NameFrom(); }
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
