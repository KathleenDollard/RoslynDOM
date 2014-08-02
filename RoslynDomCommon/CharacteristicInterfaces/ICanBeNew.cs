namespace RoslynDom.Common
{
    public interface ICanBeNew : IDom
    {
        bool IsNew { get; set; }
    }
}
