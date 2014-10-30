using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

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
   public class RDomDetailBlockStart : RDomDetail<IDetailBlockStart>, IDetailBlockStart
   {

      public RDomDetailBlockStart( SyntaxTrivia trivia,string text)
          : base(StemMemberKind.RegionStart, MemberKind.RegionStart,trivia)
      {
         _text = text;
      }

      internal RDomDetailBlockStart(RDomDetailBlockStart oldRDom)
          : base(oldRDom)
      {
         _text = oldRDom.Text;
         // Copy must be completed through the region end and only when the parent is also copied.
         // This method leaves things in an unstable state until the RegionEnd runs
         BlockEnd = oldRDom.BlockEnd; // temporary until the new one is created
      }

      public IDetailBlockEnd BlockEnd { get; internal set; }

      public string BlockStyleName
      { get { return "region"; } }

      public IEnumerable<IDom> BlockContents
      {
         get
         {
            if (BlockEnd.Parent != Parent) return null;
            var parent = Parent;
            while (!(parent is IContainer || parent is object))
            { parent = parent.Parent; }
            var parentAsContainer = parent as IContainer;
            var ret = parentAsContainer.GetMembers()
                        .FollowingSiblings(this);
             ret = ret.PreviousSiblings(BlockEnd);
            return ret.ToList();
         }
      }

      private string _text;
      public string Text
      {
         get { return _text; }
         set { SetProperty(ref _text, value); }
      }
   }
}
