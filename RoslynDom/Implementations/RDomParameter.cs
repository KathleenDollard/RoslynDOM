using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    public class RDomParameter : RDomBase<IParameter, IParameterSymbol>, IParameter
    {
        private AttributeList _attributes = new AttributeList();

        public RDomParameter(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomParameter(RDomParameter oldRDom)
           : base(oldRDom)
        {
                        Attributes.AddOrMoveAttributeRange( oldRDom.Attributes.Select(x=>x.Copy()));
            Type = oldRDom.Type;
            IsOut = oldRDom.IsOut;
            IsRef = oldRDom.IsRef;
            IsParamArray = oldRDom.IsParamArray;
            IsOptional = oldRDom.IsOptional;
            Ordinal = oldRDom.Ordinal;
        }
        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }


        public AttributeList Attributes
        { get { return _attributes; } }

        public IReferencedType Type { get; set; }

        public bool IsOut { get; set; }

        public bool IsRef { get; set; }

        public bool IsParamArray { get; set; }

        public bool IsOptional { get; set; }

        public int Ordinal { get; set; }

        // TODO: Default Values for parameters!!!

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            {
                return Type.QualifiedName;
            }
            return base.RequestValue(name);
        }
    }


}
