using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IRoot : IDom, IStemContainer
    {
        IEnumerable<IClass> RootClasses { get; }
        IEnumerable<IInterface > RootInterfaces { get; }
        IEnumerable<IEnum> RootEnums { get; }
        IEnumerable<IStructure > RootStructures { get; }

        bool HasSyntaxErrors { get; }
    }
}
