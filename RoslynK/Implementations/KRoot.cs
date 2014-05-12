using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{
    public class KRoot : KBaseStemContainer<CompilationUnitSyntax>, IRoot
    {

        internal KRoot(CompilationUnitSyntax rawItem,
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

          public override string BestInContextName
        {
            get { return Name; }
        }
    }
}
