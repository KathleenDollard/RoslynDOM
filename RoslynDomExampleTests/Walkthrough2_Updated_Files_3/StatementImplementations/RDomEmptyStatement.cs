using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
    public class RDomEmptyStatement : RDomBase<IEmptyStatement, ISymbol>, IEmptyStatement
    {
public RDomEmptyStatement()
: this(null, null, null)
{
    NeedsFormatting = true;
}
        public RDomEmptyStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomEmptyStatement(RDomEmptyStatement oldRDom)
            : base(oldRDom)
        { }
    }
}
