namespace RoslynDom.Common
{
    public enum StemMemberKind
    {
        Unknown = 0,
        UsingDirective,
        Namespace,
        Class,
        Structure,
        Enum,
        Interface,
        Whitespace,
        Comment,
        Invalid, 
        RegionStart,
        RegionEnd,
        PublicAnnotation

    }
}
