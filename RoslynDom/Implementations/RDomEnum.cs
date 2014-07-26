using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomEnum : RDomBase<IEnum, ISymbol>, IEnum
    {
        private AttributeList _attributes = new AttributeList();
        private RDomList<IEnumValue> _values;

        public RDomEnum(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomEnum(RDomEnum oldRDom)
             : base(oldRDom)
        {
            Initialize();
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            var newValues = RoslynDomUtilities.CopyMembers(oldRDom._values);
            Values.AddOrMoveRange(newValues);
            AccessModifier = oldRDom.AccessModifier;
            UnderlyingType = oldRDom.UnderlyingType;
        }

        protected override void Initialize()
        {
            _values = new RDomList<IEnumValue>(this);
            base.Initialize();
        }

        public AttributeList Attributes
        { get { return _attributes; } }

        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }

        public string QualifiedName
        { get { return RoslynUtilities.GetQualifiedName(this); } }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

        public bool IsNested
        { get { return (Parent is IType); } }
        public IType ContainingType
        { get { return Parent as IType; } }

        public RDomList<IEnumValue> Values
        { get { return _values; } }

        public AccessModifier AccessModifier { get; set; }

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
