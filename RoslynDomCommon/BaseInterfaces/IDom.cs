using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IDom : IHasSameIntentMethod
   {
      object RawItem { get; }
      object OriginalRawItem { get; }

      bool NeedsFormatting { get; set; }
      Whitespace2Collection Whitespace2Set { get; }
      void EnsureNewLineAfter();
      void EnsureLeading(string whitespace);

      bool Matches(IDom other);
      IDom Parent { get; }
      IEnumerable<IDom> Children { get; }

      IEnumerable<IDom> Ancestors { get; }
      IEnumerable<IDom> AncestorsAndSelf { get; }
      IEnumerable<IDom> Descendants { get; }
      IEnumerable<IDom> DescendantsAndSelf { get; }

      string ReportHierarchy();

      object RequestValue(string propertyName);

      PublicAnnotationList PublicAnnotations { get; }
   }

   public interface IDom<T> : IDom
       where T : IDom<T>
   {
      T Copy();
   }
}