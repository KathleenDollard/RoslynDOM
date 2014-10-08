using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomEnumMember : RDomBase<IEnumMember, ISymbol>, IEnumMember
   {
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomEnumMember(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
      internal RDomEnumMember(RDomEnumMember oldRDom)
           : base(oldRDom)
      {
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         if (oldRDom.Expression != null)
         { Expression = oldRDom.Expression.Copy(); }
         Name = oldRDom.Name;
      }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      [Required]
      public string Name { get; set; }
      public IExpression Expression { get; set; }
   }
}
