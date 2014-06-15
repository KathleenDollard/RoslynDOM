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
    public abstract class RDomBaseType<T> : RDomSyntaxNodeBase<T, INamedTypeSymbol>, IType
        where T : SyntaxNode
    {
        private IEnumerable<ITypeMember> _members;
        private IEnumerable<IAttribute> _attributes;
        internal RDomBaseType(
            T rawItem,
            IEnumerable<ITypeMember> members)
            : base(rawItem)
        {
            _members = members;
        }

        public IEnumerable<ITypeMember> Members
        {
            get
            { return _members; }
        }
        public IEnumerable<IMethod> Methods
        {
            get
            { return Members.OfType<IMethod>(); }
        }
        public IEnumerable<IProperty> Properties
        {
            get
            { return Members.OfType<IProperty>(); }
        }

        public IEnumerable<IField> Fields
        {
            get
            { return Members.OfType<IField>(); }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {                return GetAttributes();            }
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

