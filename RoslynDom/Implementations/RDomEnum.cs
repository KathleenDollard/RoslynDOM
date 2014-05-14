using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;


namespace RoslynDom.Implementations
{
    public class RDomEnum : RDomSyntaxNodeBase<EnumDeclarationSyntax>, IEnum
    {
        internal RDomEnum(EnumDeclarationSyntax rawItem) : base(rawItem) { }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get { return TypedRawItem.Identifier.ToString(); }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.Identifier.QualifiedNameFrom(); }
        }

        public override string BestInContextName
        {
            get { return TypedRawItem.Identifier.BestInContextNameFrom(); }
        }

        public string OriginalName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
