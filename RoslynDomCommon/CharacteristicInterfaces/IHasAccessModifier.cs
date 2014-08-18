namespace RoslynDom.Common
{
    public interface IHasAccessModifier : IDom
    {
        AccessModifier AccessModifier { get; set; }
        AccessModifier DeclaredAccessModifier { get; set; }
    }
}
