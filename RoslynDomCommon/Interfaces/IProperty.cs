using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IProperty : IPropertyOrMethod<IProperty >
    {
        IReferencedType PropertyType { get; set; }
        bool CanGet { get; set; }
        bool CanSet { get; set; }
        IAccessor GetAccessor { get; set; }
        IAccessor SetAccessor { get; set; }
    }
}