namespace RoslynDom.Common
{
   public enum StatementKind
   {
      Unknown = 0,
      Block,
      Do,
      ForEach,
      For,           // not suporting multiple initializers or incrementors right now
      If,
      Empty,
      Return,
      Declaration,
      Try,
      While,
      Using,

      //Else,    characteristic of If
      // Catch,  characteristic of Try
      //Switch,   // can this be handled as a special case of if?
      Break,

      Continue,
      Throw,

      //Expression statements, // break this appart into two kinds
      Invocation,

      Assignment,

      //Special, // (platform or lanuguage specific)
      //Yield, Split into YieldBreak and YieldReturn (expression)

      Checked,
      Lock,

      // Planning to avoid unless someone has a scenario. Fixed and unsafe could be characteristics, perhaps
      //Unsafe,
      //Fixed,
      //Goto,
      //Labeled,
   }
}