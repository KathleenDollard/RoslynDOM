using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{

    public class KNamespace : KBaseStemContainer<NamespaceDeclarationSyntax>, INamespace
    {
        internal KNamespace(NamespaceDeclarationSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings)
            : base(rawItem, members, usings)
        { }

        public override string Name
        {
            get
            {
                return this.Symbol.Name;
            }
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
