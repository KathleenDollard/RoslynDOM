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

      [Required]
      public string Name { get; set; }
      [Required]
      public IReferencedType Type { get; set; }
      public int Ordinal { get; set; }
      public bool IsOut { get; set; }
      public bool IsRef { get; set; }
      public bool IsParamArray { get; set; }
      public bool IsOptional { get; set; }
      public object DefaultValue { get; set; }
      public LiteralKind DefaultValueType { get; set; }
      public string DefaultConstantIdentifier { get; set; }
   }
}
