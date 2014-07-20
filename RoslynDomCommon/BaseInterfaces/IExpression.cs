using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IExpression : IDom<IExpression>
    {
        string Expression { get; set; } // at present this would be a rathole, particularly between languages
        ExpressionType ExpressionType { get; set; }
    }
}
