namespace RoslynDom.Common
{
    public enum StatementKind
    {
        Unknown = 0,
        Block,
        Do,
        ForEach,
        For,
        While,
        If,
        Else,
        Empty,
        Return,
        LocalDeclaration,
        Try,

        //Using,   make characteristic of block
        Break,    // probably have to support
        Continue, // probably have to support
        Throw,    // probably have to support
        Switch,   // can this be handled as a special case of if?

        //Expression, // break this appart into two kinds
        Invocation,
        Assignment,

        Special
        //Checked, 
        //Lock,
        //Unsafe,
        //Yield,
        //Fixed,
        //Goto,
        //Labeled,

    }
}
