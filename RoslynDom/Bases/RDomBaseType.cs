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
        private MemberType _memberType;
        private StemMemberType _stemMemberType;

        internal RDomBaseType(
            T rawItem,
            MemberType memberType,
            StemMemberType stemMemberType,
            IEnumerable<ITypeMember> members)
            : base(rawItem)
        {
            _members = members;
            _memberType = memberType;
            _stemMemberType = stemMemberType;
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
            { return GetAttributes(); }
        }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }

        public MemberType MemberType
        {
            get
            { return _memberType; }
        }

        public StemMemberType StemMemberType
        {
            get
            { return _stemMemberType; }

        }
    }
}

