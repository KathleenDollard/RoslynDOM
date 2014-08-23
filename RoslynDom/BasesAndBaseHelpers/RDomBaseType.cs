﻿using System;
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
        private RDomList<ITypeMemberCommentWhite> _members;
        private RDomList<IReferencedType> _implementedInterfaces;
        private IEnumerable<IReferencedType> _allImplementedInterfaces;
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
            _memberKind = oldRDom._memberKind;
            _stemMemberKind = oldRDom._stemMemberKind;
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            AccessModifier = oldRDom.AccessModifier;
            DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
            MembersAll.AddOrMoveRange(RoslynDomUtilities.CopyMembers(oldRDom._members));
            TypeParameters.AddOrMoveRange(RoslynDomUtilities.CopyMembers(oldRDom._typeParameters));

            _implementedInterfaces.AddOrMoveRange(oldRDom._implementedInterfaces.Select(x => x.Copy()));
            // TODO: _allImplementedInterfaces = oldRDom._allImplementedInterfaces.Select(x => x.Copy());
        }

        protected override void Initialize()
        {
            base.Initialize();
            _members = new RDomList<ITypeMemberCommentWhite>(this);
            _typeParameters = new RDomList<ITypeParameter>(this);
            _implementedInterfaces = new RDomList<IReferencedType>(this);
            var thisAsHasInterfaces = this as IHasImplementedInterfaces;
            var typeSymbol = Symbol as ITypeSymbol;
            if (typeSymbol == null) throw new NotImplementedException();
            // TODO: _allImplementedInterfaces = typeSymbol.AllInterfaces
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

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }

        public string QualifiedName
        { get { return RoslynUtilities.GetQualifiedName(this); } }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

        public string ContainingTypeName
        { get { return RoslynDomUtilities.GetContainingTypeName(this.Parent); } }

        public IType ContainingType
        { get { return Parent as IType; } }

        public bool IsNested
        { get { return (Parent is IType); } }

        public RDomList<ITypeParameter> TypeParameters
        {
            get
            {
                return _typeParameters;
            }
        }

        public RDomList<ITypeMemberCommentWhite> MembersAll
        { get { return _members; } }

        public IEnumerable<ITypeMember> Members
        { get { return _members.OfType<ITypeMember>().ToList(); } }

        public IEnumerable<IMethod> Methods
        { get { return Members.OfType<IMethod>().ToList(); } }

        public IEnumerable<IProperty> Properties
        { get { return Members.OfType<IProperty>().ToList(); } }

        public IEnumerable<IField> Fields
        { get { return Members.OfType<IField>().ToList(); } }

        // This is not yet editale because it is non-trivial to ensure 
        // correct interface usage (appearing once, etc). These semantics
        // may also change as "all" is confusing with other use in RoslynDon
        public RDomList<IReferencedType> ImplementedInterfaces
        { get { return _implementedInterfaces; } }
        public IEnumerable<IReferencedType> AllImplementedInterfaces
        {
            get
            {
                // TODO: Figure out all interfaces or don't support
                return null;
            }
        }


        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }

        public AccessModifier DeclaredAccessModifier { get; set; }

        public MemberKind MemberKind
        { get { return _memberKind; } }

        public StemMemberKind StemMemberKind
        { get { return _stemMemberKind; } }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }
    }
}

