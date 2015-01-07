namespace RoslynDom.Common
{
   //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
   //   "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
   //   Justification = "Because this represents a namespace, it's an appropriate name")]
   public interface IHasNamespace : IHasName, IDom
   {
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
          "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Namespace",
          Justification = "Because this represents a namespace, seems appropriate")]
      string Namespace { get; }

      string QualifiedName { get; }
   }
}