using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;


namespace RoslynDom
{
    public class RDomEnum : RDomSyntaxNodeBase<EnumDeclarationSyntax>, IEnum
    {
        internal RDomEnum(EnumDeclarationSyntax rawItem) : base(rawItem) { }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return this.AttributesFrom();
            }
        }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }
    }
}
