using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IHasName
    {
        string Name { get; set; }
        string OuterName { get; }
    }
}
