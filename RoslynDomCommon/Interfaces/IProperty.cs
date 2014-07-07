using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IProperty : IPropertyOrMethod<IProperty >
    {
        IReferencedType PropertyType { get; }
        bool CanGet { get; }
        bool CanSet { get; }
        IAccessor GetAccessor { get; }
        IAccessor SetAccessor { get; }
    }
}