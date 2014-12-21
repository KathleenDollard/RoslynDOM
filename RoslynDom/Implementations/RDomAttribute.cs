using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
       "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
               Justification = "Because this represents an attribute, it's an appropriate name")]
   public class RDomAttribute : RDomBase<IAttribute, ISymbol>, IAttribute
   {
      private RDomCollection<IAttributeValue> _attributeValues;

      public RDomAttribute(IDom parent, string name)
      : base( parent)
      {
         Initialize();
         _name = name;
      }

      public RDomAttribute(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
         Initialize();
      }

      private void Initialize()
      {
         _attributeValues = new RDomCollection<IAttributeValue>(this);
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomAttribute(RDomAttribute oldRDom)
          : base(oldRDom)
      {
         _name = oldRDom.Name;
         _attributeValues = oldRDom.AttributeValues.Copy(this);
      }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public void RemoveAttributeValue(IAttributeValue attributeValue)
      { _attributeValues.Remove(attributeValue); }

      public void AddOrMoveAttributeValue(IAttributeValue attributeValue)
      { _attributeValues.AddOrMove(attributeValue); }

      public RDomCollection <IAttributeValue> AttributeValues
      { get { return _attributeValues; } }
   }
}
