namespace RoslynDom.Common
{
    public interface IHasReturnType : IDom
    {
        IReferencedType  ReturnType { get; set; }
    }
}
