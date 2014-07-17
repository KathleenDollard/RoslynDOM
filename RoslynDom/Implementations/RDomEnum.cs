using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomEnum : RDomBase<IEnum, ISymbol>, IEnum
    {
        private AttributeList _attributes = new AttributeList();

        public RDomEnum(SyntaxNode rawItem, SemanticModel model)
           : base(rawItem, model)
        { }

        internal RDomEnum(RDomEnum oldRDom)
             : base(oldRDom)
        {
                        Attributes.AddOrMoveAttributeRange( oldRDom.Attributes.Select(x=>x.Copy()));
            AccessModifier = oldRDom.AccessModifier;
            UnderlyingType = oldRDom.UnderlyingType;
        }
        public AttributeList Attributes
        { get { return _attributes; } }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

        public IType ContainingType { get; set; }

        public string QualifiedName
        {
            get { return GetQualifiedName(); }
        }

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
    }
}
