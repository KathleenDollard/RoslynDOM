using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomParameter : RDomBase<IParameter, IParameterSymbol>, IParameter
   {
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomParameter(string name, string typeName,
                      int ordinal = 0, bool isOut = false, bool isRef = false,
                      bool isParamArray = false)
          : this(name, new RDomReferencedType(typeName, true), null, 
                LiteralKind.Unknown, null, ordinal, isOut, isRef, isParamArray)
      { }

      public RDomParameter(string name, IReferencedType type,
                      int ordinal = 0, bool isOut = false, bool isRef = false,
                      bool isParamArray = false)
         : this(name, type, null, LiteralKind.Unknown, null, ordinal, isOut, isRef, isParamArray)
      { }

      public RDomParameter(string name, string typeName,
                     object defaultValue, LiteralKind defaultValueType, string defaultConstantIdentifier,
                     int ordinal = 0, bool isOut = false, bool isRef = false,
                     bool isParamArray = false)
         : this(name, new RDomReferencedType(typeName, true), defaultValue, defaultValueType, defaultConstantIdentifier, ordinal, isOut, isRef, isParamArray)
      { }

      public RDomParameter(string name, IReferencedType type,
                   object defaultValue, LiteralKind defaultValueType, string defaultConstantIdentifier,
                   int ordinal = 0, bool isOut = false, bool isRef = false,
                   bool isParamArray = false)
         : this(null, null, null)
      {
         Name = name;
         Type = type;
         Ordinal = ordinal;
         IsOut = isOut;
         IsRef = isRef;
         IsParamArray = isParamArray;
         IsOptional = defaultValueType != LiteralKind.Unknown;
         if (IsOptional)
         {
            DefaultValue = defaultValue;
            DefaultValueType = defaultValueType;
            DefaultConstantIdentifier = defaultConstantIdentifier;
         }
      }

      public RDomParameter(SyntaxNode rawItem, IDom parent, SemanticModel model)
             : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomParameter(RDomParameter oldRDom)
          : base(oldRDom)
      {
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         Type = oldRDom.Type;
         IsOut = oldRDom.IsOut;
         IsRef = oldRDom.IsRef;
         IsParamArray = oldRDom.IsParamArray;
         IsOptional = oldRDom.IsOptional;
         DefaultValue = oldRDom.DefaultValue;
         DefaultValueType = oldRDom.DefaultValueType;
         Ordinal = oldRDom.Ordinal;
      }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      private IReferencedType _type;
      [Required]
      public IReferencedType Type
      {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }

      private int _ordinal;
      public int Ordinal
      {
         get { return _ordinal; }
         set { SetProperty(ref _ordinal, value); }
      }

      private bool _isOut;
      public bool IsOut
      {
         get { return _isOut; }
         set { SetProperty(ref _isOut, value); }
      }

      private bool _isRef;
      public bool IsRef
      {
         get { return _isRef; }
         set { SetProperty(ref _isRef, value); }
      }

      private bool _isParamArray;
      public bool IsParamArray
      {
         get { return _isParamArray; }
         set { SetProperty(ref _isParamArray, value); }
      }

      private bool _isOptional;
      public bool IsOptional
      {
         get { return _isOptional; }
         set { SetProperty(ref _isOptional, value); }
      }

      private object _defaultValue;
      public object DefaultValue
      {
         get { return _defaultValue; }
         set { SetProperty(ref _defaultValue, value); }
      }

      private LiteralKind _defaultValueType;
      public LiteralKind DefaultValueType
      {
         get { return _defaultValueType; }
         set { SetProperty(ref _defaultValueType, value); }
      }

      private string _defaultConstantIdentifier;
      public string DefaultConstantIdentifier
      {
         get { return _defaultConstantIdentifier; }
         set { SetProperty(ref _defaultConstantIdentifier, value); }
      }
   }
}
