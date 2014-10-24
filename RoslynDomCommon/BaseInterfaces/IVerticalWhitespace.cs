using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public interface IVerticalWhitespace : IDetail<IVerticalWhitespace>
    {
        int Count { get; set; }
        bool IsElastic { get; set; }
    }
}
