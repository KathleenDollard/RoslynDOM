using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBaseType<T>
        : RDomBase<T, INamedTypeSymbol>, IType<T>, ITypeMemberContainer
        where T : class, IType<T>
    {
        private RDomList<ITypeMember> _members;
        private MemberKind _memberKind;        // This should remain readonly
        private StemMemberKind _stemMemberKind;// This should remain readonly
        private RDomList<ITypeParameter> _typeParameters;
        private AttributeList _attributes = new AttributeList();

        internal RDomBaseType(
              SyntaxNode rawItem,
              IDom parent,
              SemanticModel model,
              MemberKind memberKind,
              StemMemberKind stemMemberKind)
           : base(rawItem, parent, model)
        {
            _memberKind = memberKind;
            _stemMemberKind = stemMemberKind;
        }


        internal RDomBaseType(T oldIDom)
             : base(oldIDom)
        {
            var oldRDom = oldIDom as RDomBaseType<T>;
            Initialize();
            _memberKind = oldRDom._memberKind;
            _stemMemberKind = oldRDom._stemMemberKind;
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            AccessModifier = oldRDom.AccessModifier;
            var newMembers = RoslynDomUtilities.CopyMembers(oldRDom._members);
            MembersAll.AddOrMoveRange(newMembers);
            var newTypeParameters = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
            TypeParameters.AddOrMoveRange(newTypeParameters);
        }

        protected override void Initialize()
        {
            base.Initialize();
            _members = new RDomList<ITypeMember>(this);
            _typeParameters = new RDomList<ITypeParameter>(this);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = base.Children.ToList();
                list.AddRange(_members);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = base.Descendants.ToList();
                foreach (var member in _members)
                { list.AddRange(member.DescendantsAndSelf); }
                return list;
            }
        }

        public string Name { get; set; }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

        public IType ContainingType { get; set; }

        public string QualifiedName
        { get { return GetQualifiedName(); } }

        public RDomList<ITypeParameter> TypeParameters
        {
            get
            {
                return _typeParameters;
            }
        }

        public RDomList<ITypeMember> MembersAll
        { get { return _members; } }

        public IEnumerable<ITypeMember> Members
        { get { return _members; } }

        public IEnumerable<IMethod> Methods
        { get { return Members.OfType<IMethod>(); } }

        public IEnumerable<IProperty> Properties
        { get { return Members.OfType<IProperty>(); } }

        public IEnumerable<IField> Fields
        { get { return Members.OfType<IField>(); } }
        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }

        public MemberKind MemberKind
        { get { return _memberKind; } }

        public StemMemberKind StemMemberKind
        { get { return _stemMemberKind; } }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }
    }
}

