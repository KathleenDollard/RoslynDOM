using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttributeValue
        : RDomBase<IAttributeValue, ISymbol>, IAttributeValue
    {

        public RDomAttributeValue(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomAttributeValue(
             RDomAttributeValue oldRDom)
            : base(oldRDom)
        {
            ValueType = oldRDom.ValueType;
            Value = oldRDom.Value;
            // TODO: manage type
            Type = oldRDom.Type;
            Style = oldRDom.Style;
        }

        public string Name { get; set; }

        public object Value { get; set; }

        public LiteralKind ValueType { get; set; }

        public AttributeValueStyle Style { get; set; }

        public Type Type { get; set; }
    }
}
