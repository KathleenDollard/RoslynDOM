using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInterface : RDomBaseType<InterfaceDeclarationSyntax>, IInterface
    {
        internal RDomInterface(InterfaceDeclarationSyntax rawItem,
            IEnumerable<ITypeMember> members) : base(rawItem, members)
        { }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return this.AttributesFrom();
            }
        }
    }
}
