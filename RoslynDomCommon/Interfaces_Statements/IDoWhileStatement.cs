using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IDoWhileStatement :ILoop<IDoWh>
    {
        IExpression Condition { get; set; }
        bool HasBlock { get; set; }
        bool TestAtEnd { get; set; }
    }
}
