namespace RoslynDom.Common
{
   public interface IHasStructuredDocumentation : IDom
   {
      IStructuredDocumentation StructuredDocumentation { get; set; }

      /// <summary>
      ///
      /// </summary>
      /// <remarks>
      /// NOTE TO IMPLEMENTORS: This value should simply be copied to and from the contained StructuredDocumentation Description.
      /// </remarks>
      string Description { get; set; }
   }
}