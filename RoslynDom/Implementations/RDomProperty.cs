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
    public class RDomProperty : RDomSyntaxNodeBase<PropertyDeclarationSyntax>, IProperty
    {
        internal RDomProperty(PropertyDeclarationSyntax rawItem) : base(rawItem) { }

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

        public override string QualifiedName
        {
            get
            {
                // TODO: Manage static member's qualified names
                throw new InvalidOperationException("You can't get qualified name for an instance property");
            }
        }


        public override string Namespace
        {
            get
            {
                throw new InvalidOperationException("You can't get namespace for an instance method");
            }
        }

        public IReferencedType ReturnType
        {
                get
                {
                    var localSymbol = Symbol as IPropertySymbol ;
                    return new RDomReferencedType(localSymbol.DeclaringSyntaxReferences , localSymbol.Type);
                }
            }

    }
}
