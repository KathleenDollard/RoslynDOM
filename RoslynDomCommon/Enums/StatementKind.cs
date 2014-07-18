namespace RoslynDom.Common
{
    public enum StatementKind
    {
        Unknown = 0,
        Block,      // done
        Do,         
        ForEach,    
        For,                 // not suporting multiple initializers or incrementors right now 
        If,         // done
        Empty,      
        Return,     // done
        Declaration,// done
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
        Invocation,  // done
        Assignment,  // done

        Special // (platform or lanuguage specific)
        //Yield, Split into YieldBreak and YieldReturn (expression)

        //Checked ( characteristic of block? )
        //Lock,   ( characteristic of block? )

        // Planning to avoid unless someone has a scenario. Fixed and unsafe could be characteristics, perhaps
        //Unsafe,
        //Fixed,
        //Goto,
        //Labeled,
    }
}
