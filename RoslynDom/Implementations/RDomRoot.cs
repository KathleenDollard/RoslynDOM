using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomRoot : RDomBaseStemContainer<CompilationUnitSyntax>, IRoot
    {

        internal RDomRoot(CompilationUnitSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings)
        : base(rawItem, members, usings)
        { }

        public override string Name
        {
            get { return "Root"; }
        }

        public override string QualifiedName
        {
            get { return Name; }
        }

    }
}
