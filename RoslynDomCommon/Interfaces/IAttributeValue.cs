using System;

namespace RoslynDom.Common
{
   public interface IAttributeValue : IDom<IAttributeValue>, IHasName, IMisc
   {
      object Value { get; set; }

      string ValueConstantIdentifier { get; set; }

      LiteralKind ValueType { get; set; }

      AttributeValueStyle Style { get; set; }

      Type Type { get; set; }
   }

   public interface IAttributeValueConstant : IAttributeValue
   {
   }

   public interface IAttributeValueInteger : IAttributeValue
   {
      new int Value { get; set; }
   }
}