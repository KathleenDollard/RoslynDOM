using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynK
{
    public class KStructure : KBaseClassOrStructure<StructDeclarationSyntax>, IStructure
    {
        internal KStructure(StructDeclarationSyntax rawItem,
           IEnumerable<ITypeMember> members)
            : base(rawItem, members)
        { }

        public override string Name
        {
            get { return TypedRawItem.Identifier.NameFrom(); }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.Identifier.QNameFrom(); }
        }

        public override string BestInContextName
        {
            get { return TypedRawItem.Identifier.BestInContextNameFrom(); }
        }
    }
}
