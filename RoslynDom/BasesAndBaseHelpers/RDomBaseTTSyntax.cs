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
        where T : class, IType<T>
    {
        private IEnumerable<ITypeMember> _members;
        private MemberType _memberType;
        private StemMemberType _stemMemberType;
        private IEnumerable<ITypeParameter> _typeParameters;

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
            Initialize();
        }

        internal RDomBaseType(T oldIDom)
             : base(oldIDom)
        {
            var oldRDom = oldIDom as RDomBaseType<T, TSyntax>;
            _memberType = oldRDom._memberType;
            _stemMemberType = oldRDom._stemMemberType;
            AccessModifier = oldRDom.AccessModifier;
            _members = RDomBase<IClass>.CopyMembers(oldRDom._members.OfType<RDomClass>());
            _members = _members.Union(RDomBase<IStructure>.CopyMembers(oldRDom._members.OfType<RDomStructure>()));
            _members = _members.Union(RDomBase<IInterface>.CopyMembers(oldRDom._members.OfType<RDomInterface>()));
            _members = _members.Union(RDomBase<IEnum>.CopyMembers(oldRDom._members.OfType<RDomEnum>()));
            _members = _members.Union(RDomBase<IProperty>.CopyMembers(oldRDom._members.OfType<RDomProperty>()));
            _members = _members.Union(RDomBase<IMethod>.CopyMembers(oldRDom._members.OfType<RDomMethod>()));
            _members = _members.Union(RDomBase<IField>.CopyMembers(oldRDom._members.OfType<RDomField>()));
            _typeParameters = RDomBase<ITypeParameter>
                                .CopyMembers(oldRDom._typeParameters.Cast<RDomTypeParameter>());
        }

        protected override void Initialize()
        {
            base.Initialize();
            _typeParameters = this.TypedSymbol.TypeParametersFrom();
            AccessModifier = (AccessModifier)Symbol.DeclaredAccessibility;
        }

        protected override bool CheckSameIntent(T other, bool includePublicAnnotations)
        {
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
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

        public void AddMember(ITypeMember newMember)
        {
            _members = _members.Concat(new ITypeMember[] { newMember });
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }


        public MemberType MemberType
        { get { return _memberType; } }

        public StemMemberType StemMemberType
        { get { return _stemMemberType; } }
    }
}

