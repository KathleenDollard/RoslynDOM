namespace RoslynDom.Common
{
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
       "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Operator",
       Justification = "This is an enum of operators, so name seems appropriate")]
   public enum Operator
   {
      Unknown = 0,
      Plus,
      Minus,
      Asterisk,
      Slash,
      Percent,
      Ampersand,
      Bar,
      Caret,
      LessThanLessThan,
      GreaterThanGreaterThan,
      EqualsEquals,
      ExclamationEquals,
      GreaterThan,
      LessThan,
      GreaterThanEquals,
      LessThanEquals
   }
}