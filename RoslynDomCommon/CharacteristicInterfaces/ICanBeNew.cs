namespace RoslynDom.Common
{
    public interface ICanBeNew : IDom
    {
        // While all type member OO items (property and method) are CanBeNew, not all 
        // CanBeNew are OO (fields)

        // I'm starting with C# where most OO concepts are declared (not implicit)
        // New however is implicit. This raises issues for language agnostic-ness
        // I am not yet including a declared/real pattern because I'm not ready to 
        // commit to them being in sync. For now, for C#, New is the declared New
        bool IsNew { get; set; }

    }
}
