using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Field assignments in the form "Type x, y, z" are not yet supported 
    /// and when they are they will be loaded as separate fields (rather 
    /// obviously). At that point, the variable declaration will need to be held in
    /// the class. 
    /// </remarks>
    public class RDomField : RDomBase<IField, IFieldSymbol>, IField
    {
        private AttributeList _attributes = new AttributeList();

        public RDomField(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
        }

        internal RDomField(RDomField oldRDom)
            : base(oldRDom)
        {
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsStatic = oldRDom.IsStatic;
            IsReadOnly = oldRDom.IsReadOnly;
            IsVolatile = oldRDom.IsVolatile;
            IsNew = oldRDom.IsNew;
        }

        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }


        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }

        public IReferencedType ReturnType { get; set; }

        public bool IsStatic { get; set; }
        public bool IsNew { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsVolatile { get; set; }
        public IExpression Initializer { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.Field; }
        }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            { return ReturnType.QualifiedName; }
            return base.RequestValue(name);
        }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get ; set;}
    }
}
