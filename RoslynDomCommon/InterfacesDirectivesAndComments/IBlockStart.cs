using System.Collections.Generic;

namespace RoslynDom.Common
{
   /// <summary>
   ///
   /// </summary>
   public interface IDetailBlockStart : IDetail<IDetailBlockStart>, IHasGroup
   {
      IDetailBlockEnd BlockEnd { get; }

      string Text { get; set; }

      string BlockStyleName { get; }

      IEnumerable<IDom> BlockContents { get; }
   }
}