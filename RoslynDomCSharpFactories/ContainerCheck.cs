using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
 
    public class ContainerChecker : IContainerCheck
    {
    
        public bool ContainerCheck()
        {
          return  RDomCSharp.Factory.ContainerCheck();
        }
    }
}
