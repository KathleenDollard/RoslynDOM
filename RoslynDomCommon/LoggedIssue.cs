namespace Roslyn.Common
{
   public class LoggedIssue
   {
      public LoggedIssue(string message)
      {
         Message = message;
      }
      public string Message { get; }
   }
}