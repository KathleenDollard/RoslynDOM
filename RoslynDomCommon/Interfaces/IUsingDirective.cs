namespace RoslynDom.Common
{
    public  interface IUsingDirective : IDom<IUsingDirective>, IStemMember
    {
        string Alias { get; set; }
    }
}
