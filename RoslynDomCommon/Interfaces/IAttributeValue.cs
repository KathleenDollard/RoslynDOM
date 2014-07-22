using System;

namespace RoslynDom.Common
{
    public interface IAttributeValue : IDom<IAttributeValue>, IHasName
    {
        LiteralKind ValueType { get; set; }
        object Value { get; set; }
        AttributeValueStyle Style { get; set; }
        Type Type { get; set; }
    }
}
