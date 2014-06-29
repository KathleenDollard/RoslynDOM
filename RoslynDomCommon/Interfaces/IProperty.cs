namespace RoslynDom.Common
{
    public interface IProperty : IPropertyOrMethod
    {
        bool CanGet { get; }
        bool CanSet { get; }
    }
}