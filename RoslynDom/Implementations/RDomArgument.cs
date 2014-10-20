using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{

   /// <summary></summary>
   /// <returns></returns>
   /// <remarks>
   /// Field assignments in the form "Type x, y, z" are not yet supported
   /// and when they are they will be loaded as separate fields (rather
   /// obviously). At that point, the variable declaration will need to be held in
   /// the class.
   /// </remarks>
   public class RDomArgument : RDomBase<IArgument, ISymbol>, IArgument
   {
      public RDomArgument(IExpression valueExpression, string name = null,
                     bool isRef = false, bool isOut = false)
       : this(null, null, null)
      {
         _name = name;
         _valueExpression = valueExpression;
         _isRef = isRef;
         _isOut = isOut;
      }

      public RDomArgument(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomArgument(RDomArgument oldRDom)
          : base(oldRDom)
      {
         _name = oldRDom.Name;
         _isRef = oldRDom.IsRef;
         _isOut = oldRDom.IsOut;
         _valueExpression = oldRDom.ValueExpression.Copy();
      }

      private string _name;
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      private IExpression _valueExpression;
      [Required]
      public IExpression ValueExpression
      {
         get { return _valueExpression; }
         set { SetProperty(ref _valueExpression, value); }
      }

      private bool _isRef;
      public bool IsRef
      {
         get { return _isRef; }
         set { SetProperty(ref _isRef, value); }
      }

      private bool _isOut;
      public bool IsOut
      {
         get { return _isOut; }
         set { SetProperty(ref _isOut, value); }
      }
   }
}
