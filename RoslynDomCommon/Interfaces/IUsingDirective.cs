namespace RoslynDom.Common
{
    public  interface IUsingDirective : IDom<IUsingDirective>, IStemMember
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
            "CA1716:IdentifiersShouldNotHaveIncorrectSuffix",
            Justification = "Because this represents an alias, it's an appropriate name")]
        string Alias { get; set; }
    }
}
