using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IRoot : IDom<IRoot>, IStemContainer, IHasName
    {
        IEnumerable<IClass> RootClasses { get; }
        IEnumerable<IInterface > RootInterfaces { get; }
        IEnumerable<IEnum> RootEnums { get; }
        IEnumerable<IStructure > RootStructures { get; }
        bool HasSyntaxErrors { get; }
    }
}
