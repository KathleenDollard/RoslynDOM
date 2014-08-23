using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomEnum : RDomBase<IEnum, ISymbol>, IEnum
    {
        private AttributeList _attributes = new AttributeList();
        private RDomList<IEnumMember> _values;

        public RDomEnum(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomEnum(RDomEnum oldRDom)
             : base(oldRDom)
        {
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            var newValues = RoslynDomUtilities.CopyMembers(oldRDom._values);
            Members.AddOrMoveRange(newValues);
            AccessModifier = oldRDom.AccessModifier;
            DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
            UnderlyingType = oldRDom.UnderlyingType;
        }

        protected override void Initialize()
        {
            _values = new RDomList<IEnumMember>(this);
            base.Initialize();
        }

        public AttributeList Attributes
        { get { return _attributes; } }

        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }

        public string QualifiedName
        { get { return RoslynUtilities.GetQualifiedName(this); } }

        public string ContainingTypeName
        { get { return RoslynDomUtilities.GetContainingTypeName(this.Parent); } }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

        public bool IsNested
        { get { return (Parent is IType); } }
        public IType ContainingType
        { get { return Parent as IType; } }

        public RDomList<IEnumMember> Members
        { get { return _values; } }

        public AccessModifier AccessModifier { get; set; }
        public AccessModifier DeclaredAccessModifier { get; set; }

        public IReferencedType UnderlyingType { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.Enum; }

        }

        public StemMemberKind StemMemberKind
        {
            get { return StemMemberKind.Enum; }

        }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }
    }
}
