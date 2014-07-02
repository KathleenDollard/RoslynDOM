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
    public abstract class RDomBaseType<T, TSyntax> : RDomBase<T, TSyntax, INamedTypeSymbol>, IType<T>
        where TSyntax : SyntaxNode
        where T : IType<T>
    {
        private IEnumerable<ITypeMember> _members;
        private IEnumerable<IAttribute> _attributes;
        private MemberType _memberType;
        private StemMemberType _stemMemberType;

        internal RDomBaseType(
            TSyntax rawItem,
            MemberType memberType,
            StemMemberType stemMemberType,
            IEnumerable<ITypeMember> members,
            params PublicAnnotation[] publicAnnotations)
            : base(rawItem, publicAnnotations)
        {
            _members = members;
            _memberType = memberType;
            _stemMemberType = stemMemberType;
        }

        internal RDomBaseType(T oldRDom)
             : base(oldRDom)
        { }

        public override bool SameIntent(T other, bool includePublicAnnotations)
        {
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            var otherItem = other as RDomBaseType<T, TSyntax>;
            if (!CheckSameIntentChildList(Fields, otherItem.Fields)) return false;
            if (!CheckSameIntentChildList(Properties, otherItem.Properties)) return false;
            if (!CheckSameIntentChildList(Methods, otherItem.Methods)) return false;
            return true;
        }

        public string Namespace
        { get { return GetNamespace(); } }

        public string QualifiedName
        { get { return GetQualifiedName(); } }

        public IEnumerable<ITypeMember> Members
        { get { return _members; } }

        public IEnumerable<IMethod> Methods
        { get { return Members.OfType<IMethod>(); } }

        public IEnumerable<IProperty> Properties
        { get { return Members.OfType<IProperty>(); } }

        public IEnumerable<IField> Fields
        { get { return Members.OfType<IField>(); } }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }

        public MemberType MemberType
        { get { return _memberType; } }

        public StemMemberType StemMemberType
        { get { return _stemMemberType; } }
    }
}

