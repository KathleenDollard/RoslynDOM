using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBaseType<T>
        : RDomBase<T, INamedTypeSymbol>, IType<T>, IRDomTypeMemberContainer
        where T : class, IType<T>
    {
        private IList<ITypeMember> _members = new List<ITypeMember>();
        private MemberKind _memberKind;        // This should remain readonly
        private StemMemberKind _stemMemberKind;// This should remain readonly
        private IList<ITypeParameter> _typeParameters = new List<ITypeParameter>();
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
            var newMembers = RoslynDomUtilities.CopyMembers(oldRDom._members);
            foreach (var member in newMembers)
            { AddOrMoveMember(member); }
            var newTypeParameters = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
            foreach (var typeParameter in newTypeParameters)
            { AddOrMoveTypeParameter(typeParameter); }
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

        public void RemoveMember(ITypeMember member)
        {
            if (member.Parent == null)
            { _members.Remove(member); }
            else
            { RoslynDomSymbolUtilities.RemoveMemberFromParent(this, member); }
        }

        public void AddOrMoveMember(ITypeMember member)
        {
            RoslynDomSymbolUtilities.PrepMemberForAdd(this, member);
            _members.Add(member);
        }

        public void ClearMembers()
        { _members.Clear(); }

        public void RemoveTypeParameter(ITypeParameter typeParameter)
        { _typeParameters.Remove(typeParameter); }

        public void AddOrMoveTypeParameter(ITypeParameter typeParameter)
        { _typeParameters.Add(typeParameter); }

        public void ClearTypeParameters()
        { _typeParameters.Clear(); }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

        public IType ContainingType { get; set; }

        public string QualifiedName
        { get { return GetQualifiedName(); } }

        public IEnumerable<ITypeParameter> TypeParameters
        {
            get
            {
                return _typeParameters;
            }
        }

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
    }
}

