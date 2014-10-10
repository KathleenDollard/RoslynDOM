using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomAttributeValue
       : RDomBase<IAttributeValue, ISymbol>, IAttributeValue
   {

      public RDomAttributeValue(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
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

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }
      private AttributeValueStyle _style;
      public AttributeValueStyle Style
      {
         get { return _style; }
         set { SetProperty(ref _style, value); }
      }
      private object _value;
      public object Value
      {
         get { return _value; }
         set { SetProperty(ref _value, value); }
      }
      private string _valueConstantIdentifier;
      public string ValueConstantIdentifier
      {
         get { return _valueConstantIdentifier; }
         set { SetProperty(ref _valueConstantIdentifier, value); }
      }
      private LiteralKind _valueType;
      public LiteralKind ValueType
      {
         get { return _valueType; }
         set { SetProperty(ref _valueType, value); }
      }
      public Type Type
      {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }
      private Type _type;
   }
}
