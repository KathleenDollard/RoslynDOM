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

      private string _name ;
      [Required]
      public string Name { get {return _name; }
set {SetProperty(ref _name, value); }}
      public IExpression Expression { get {return _expression; }
set {SetProperty(ref _expression, value); }}
      private IExpression _expression ;
   }
}
