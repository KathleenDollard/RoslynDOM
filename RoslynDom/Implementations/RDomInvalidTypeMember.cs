using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    public class RDomInvalidMember : RDomBase<IInvalidMember, ISymbol>, IInvalidMember
    {
        private AttributeCollection _attributes = new AttributeCollection();

        public RDomInvalidMember(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
            Initialize();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", 
            "CA1811:AvoidUncalledPrivateCode", Justification ="Called via Reflection")]
        internal RDomInvalidMember(RDomInvalidMember oldRDom)
             : base(oldRDom)
        {
            Initialize();
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            AccessModifier = oldRDom.AccessModifier;
            DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", 
            "CA1822:MarkMembersAsStatic",
            Justification ="Follows general pattern, but is currently unused, may later remove")]
        protected void Initialize()
        { }

        public string Name { get; set; }

        //public string OuterName
        //{ get { return RoslynUtilities.GetOuterName(this); } }


        public AttributeCollection Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }
        public AccessModifier DeclaredAccessModifier { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.Invalid; }
        }

        public StemMemberKind StemMemberKind
        {
            get { return StemMemberKind.Invalid; }
        }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }
    }
}
