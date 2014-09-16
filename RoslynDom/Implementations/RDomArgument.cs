using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Field assignments in the form "Type x, y, z" are not yet supported 
    /// and when they are they will be loaded as separate fields (rather 
    /// obviously). At that point, the variable declaration will need to be held in
    /// the class. 
    /// </remarks>
    public class RDomArgument : RDomBase<IArgument, ISymbol>, IArgument
    {
        public RDomArgument(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomArgument(RDomArgument oldRDom)
            : base(oldRDom)
        {
            Name = oldRDom.Name;
            IsRef = oldRDom.IsRef;
            IsOut = oldRDom.IsOut;
            ValueExpression = oldRDom.ValueExpression.Copy();
        }

        public string Name { get; set; }
        public bool IsRef { get; set; }
        public bool IsOut { get; set; }
        public IExpression ValueExpression { get; set; }
  
    }
}
