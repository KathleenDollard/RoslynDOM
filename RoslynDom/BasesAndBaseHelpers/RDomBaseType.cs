using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBaseType<T, TSyntax> : RDomBase<T, TSyntax, INamedTypeSymbol>, IType<T>, IRDomTypeContainer
        where TSyntax : SyntaxNode
        where T : class, IType<T>
    {
        private IList<ITypeMember> _members = new List<ITypeMember>();
        private MemberType _memberType;
        private StemMemberType _stemMemberType;
        private IList<ITypeParameter> _typeParameters = new List<ITypeParameter>();

        internal RDomBaseType(
            TSyntax rawItem,
            MemberType memberType,
            StemMemberType stemMemberType,
            IEnumerable<ITypeMember> members,
            params PublicAnnotation[] publicAnnotations)
            : base(rawItem, publicAnnotations)
        {
            foreach (var member in members)
            { AddOrMoveMember(member); }
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
            var newMembers = RoslynDomUtilities.CopyMembers(oldRDom._members);
            foreach (var member in newMembers)
            { AddOrMoveMember(member); }
            var newTypeParameters  = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
            foreach (var typeParameter in newTypeParameters)
            { AddTypeParameter(typeParameter); }
        }

        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = (AccessModifier)Symbol.DeclaredAccessibility;
            var newTypeParameters = this.TypedSymbol.TypeParametersFrom();
            foreach (var typeParameter in newTypeParameters)
            { AddTypeParameter(typeParameter); }
        }

         public void RemoveMember(ITypeMember member)
        { RoslynDomUtilities.RemoveMemberFromParent(this, member); }

        public void AddOrMoveMember(ITypeMember member)
        {
            RoslynDomUtilities.PrepMemberForAdd(this, member);
            _members.Add(member);
        }

        public void RemoveTypeParameter(ITypeParameter typeParameter)
        { _typeParameters.Remove(typeParameter); }

        public void AddTypeParameter(ITypeParameter typeParameter)
        {            _typeParameters.Add(typeParameter);        }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

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

        public AccessModifier AccessModifier { get; set; }

        public MemberType MemberType
        { get { return _memberType; } }

        public StemMemberType StemMemberType
        { get { return _stemMemberType; } }
    }
}

