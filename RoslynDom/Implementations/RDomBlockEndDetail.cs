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
   /// </remarks>
   public class RDomRegionEnd : RDomDetail<IDetailBlockEnd>, IDetailBlockEnd
   {
      public RDomRegionEnd(SyntaxTrivia trivia, IDetailBlockStart blockStart)
           : base(StemMemberKind.RegionEnd, MemberKind.RegionEnd, trivia)
      {
         BlockStart = blockStart;
         var bstart = blockStart as RDomDetailBlockStart;
         bstart.BlockEnd = this;
      }

      internal RDomRegionEnd(RDomRegionEnd oldRDom)
           : base(oldRDom)
      {
         // temporary value until parent is set - yes, this is very ugly. 
         oldBlockEnd = oldRDom;
      }

      private IDetailBlockEnd oldBlockEnd;
      public override IDom Parent
      {
         get { return base.Parent; }
         set
         {
            base.Parent = value;
            if (oldBlockEnd != null)
            {
               Func<RDomDetailBlockStart, bool> match = x => x.BlockEnd == oldBlockEnd;
               if (TryFindMatchingRegionStart<IStemContainer>(value, x => x.StemMembersAll, match)) return;
               if (TryFindMatchingRegionStart<ITypeMemberContainer>(value, x => x.MembersAll, match)) return;
               if (TryFindMatchingRegionStart<IStatementContainer>(value, x => x.StatementsAll, match)) return;
               oldBlockEnd = null;
            }
         }
      }


      private bool TryFindMatchingRegionStart<T>(IDom parent,
               Func<T, IEnumerable<IDom>> getMembers, Func<RDomDetailBlockStart, bool> matchPredicate)
         where T : class, IDom
      {
         var typedParent = parent as T;
         if (typedParent == null) { return false; }
         var members = getMembers(typedParent);
         var newStart = members
                        .OfType<RDomDetailBlockStart>()
                        .Where(matchPredicate)
                        .FirstOrDefault();
         if (newStart == null) throw new InvalidOperationException("Matching start region not found");
         newStart.BlockEnd = this;
         BlockStart = newStart;
         return true;
      }

      public IDetailBlockStart BlockStart { get; private set; }

      public string BlockStyleName
      { get { return "region"; } }
   }
}
