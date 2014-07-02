namespace RoslynDom.Common
{
    public interface IProperty : IPropertyOrMethod<IProperty >
    {
        bool CanGet { get; }
        bool CanSet { get; }
    }
}