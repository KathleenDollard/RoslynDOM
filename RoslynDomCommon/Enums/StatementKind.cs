namespace RoslynDom.Common
{
    public enum StatementKind
    {
        Unknown = 0,
        Block,      // done
        Do,         // done
        ForEach,    // done
        For,        // done     // not suporting multiple initializers or incrementors right now 
        If,         // done
        Empty,      
        Return,     // done
        Declaration,// done
        Try,
        While,      // done
        Using,   

        //Else,    characteristic of If
        // Catch,  characteristic of Try
        //Switch,   // can this be handled as a special case of if?
        Break,    // probably have to support
        Continue, // probably have to support
        Throw,    // probably have to support

        //Expression statements, // break this appart into two kinds
        Invocation,  // done
        Assignment,  // done

        Special // (platform or lanuguage specific)
        //Yield, Split into YieldBreak and YieldReturn (expression)

        //Checked ( characteristic of block? probably not)
        //Lock,   ( characteristic of block? probably not)

        // Planning to avoid unless someone has a scenario. Fixed and unsafe could be characteristics, perhaps
        //Unsafe,
        //Fixed,
        //Goto,
        //Labeled,
    }
}
