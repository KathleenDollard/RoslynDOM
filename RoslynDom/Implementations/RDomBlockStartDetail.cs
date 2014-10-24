using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System;

namespace RoslynDom
{
   /// <summary>
   /// 
   /// </summary>
   /// <remarks>
   /// Currently no constructor for making regions out of thin air because I haven't worked out
   /// how to match up start and end constructs. Probably a special method is needed
   /// <para>
   /// The RegionEnd property is filled when the RegionEnd is created.
   /// </para>
   /// </remarks>
   public class RDomRegionStart : RDomBase<IBlockStartDetail, ISymbol>, IBlockStartDetail
   {

      public RDomRegionStart(SyntaxNode rawItem, IDom parent, SemanticModel model, string text)
           : base(rawItem, parent, model)
      {
         _text = text;
      }

      internal RDomRegionStart(RDomRegionStart oldRDom)
           : base(oldRDom)
      {
         // Copy must be completed through the region end and only when the parent is also copied.
         // This method leaves things in an unstable state until the RegionEnd runs
         _text = oldRDom.Text;
         BlockEnd  = oldRDom.BlockEnd; // temporary until the new one is created
      }

      /// <summary>
      /// Throws exception on an attempt to copy
      /// </summary>
      /// <returns></returns>
      /// <remarks>
      /// Sorry for the brutal throwing of an exception, but what does copying a region (other
      /// than copying it as part of it's parent mean? Copying start without end breaks code.
      /// Does it mean copying everything in the region? Inclusive of the region itself?
      /// That seems useful, but doing it in this method may be surprising. 
      /// </remarks>
      public override IBlockStartDetail Copy()
      {
         throw new NotImplementedException("Can't explicitly copy regions");
      }

      private string _text;
      [Required]
      public string Text
      {
         get { return _text; }
         set { SetProperty(ref _text, value); }
      }

      public StemMemberKind StemMemberKind
      { get { return StemMemberKind.RegionStart; } }

      public MemberKind MemberKind
      { get { return MemberKind.RegionStart; } }

      public IBlockEndDetail  BlockEnd { get; internal set; }
   }
}
