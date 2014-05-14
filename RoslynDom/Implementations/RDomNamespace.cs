using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{

    public class RDomNamespace : RDomBaseStemContainer<NamespaceDeclarationSyntax>, INamespace
    {
        internal RDomNamespace(NamespaceDeclarationSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings)
            : base(rawItem, members, usings)
        { }

        public override string Name
        {
            get
            {
                return this.TypedRawItem.QualifiedNameFrom();
            }
        }

        public override string QualifiedName
        {
            get { return TypedRawItem.QualifiedNameFrom(); }
        }

        public string OriginalName
        {
            get
            {
                return this.TypedRawItem.Name.NameFrom();
            }
        }
  
    }
}
