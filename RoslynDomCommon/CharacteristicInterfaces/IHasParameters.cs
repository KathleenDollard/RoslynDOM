namespace RoslynDom.Common
{
    public interface IHasParameters : IDom
    {
        RDomList<IParameter> Parameters { get; }
    }
}
