namespace RoslynDom.Common
{
    public enum StatementKind
    {
        Unknown = 0,
        Block,
        Do,
        ForEach,
        For,
        If,
        Empty,
        Return,
        Declaration,
        Try,

        //While,   variation of Do
        //Else,    characteristic of If
        //Using,   characteristic of Block
        //Catch,   characteristic of Try
        //Switch,   // can this be handled as a special case of if?
        Break,    // probably have to support
        Continue, // probably have to support
        Throw,    // probably have to support

        //Expression statements, // break this appart into two kinds
        Invocation,
        Assignment,

        Special // (platform or lanuguage specific)
        //Checked (block)
        //Lock,
        //Yield, Split into YieldBreak and YieldReturn (expression)

        // Planning to avoid unless someone has a scenario
        //Unsafe,
        //Fixed,
        //Goto,
        //Labeled,
    }
}
