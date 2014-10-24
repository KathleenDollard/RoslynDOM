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
   public class RDomRegionEnd : RDomBase<IBlockEndDetail , ISymbol>, IBlockEndDetail
   {
      public RDomRegionEnd(SyntaxNode rawItem, IDom parent, SemanticModel model, SyntaxNode startSyntax)
           : base(rawItem, parent, model)
      {
         Func<RDomRegionStart, bool> match = x => x.TypedSyntax == startSyntax ;
         if (TryFindMatchingRegionStart<IStemContainer>(parent, x => x.StemMembersAll, match)) return;
         if (TryFindMatchingRegionStart<ITypeMemberContainer>(parent, x => x.MembersAll, match)) return;
         if (TryFindMatchingRegionStart<IStatementContainer>(parent, x => x.StatementsAll, match)) return;
      }

      internal RDomRegionEnd(RDomRegionEnd oldRDom, IDom parent)
           : base(oldRDom)
      {
         Func<RDomRegionStart, bool> match = x => x.BlockEnd == oldRDom;
         if (TryFindMatchingRegionStart<IStemContainer>(parent, x => x.StemMembersAll, match)) return;
         if (TryFindMatchingRegionStart<ITypeMemberContainer>(parent, x => x.MembersAll, match)) return;
         if (TryFindMatchingRegionStart<IStatementContainer>(parent, x => x.StatementsAll, match)) return;
      }

      private bool TryFindMatchingRegionStart<T>(IDom parent,
               Func<T, IEnumerable<IDom>> getMembers, Func<RDomRegionStart, bool> matchPredicate)
         where T : class, IDom
      {
         var typedParent = parent as T;
         if (typedParent == null) { return false; }
         var members = getMembers(typedParent);
         var newStart = members
                        .OfType<RDomRegionStart>()
                        .Where(matchPredicate)
                        .FirstOrDefault();
         if (newStart == null) throw new InvalidOperationException("Matching start region not found");
         newStart.BlockEnd = this;
         this.BlockStart  = newStart;
         return true;
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
      public override IBlockEndDetail Copy()
      {
         throw new NotImplementedException("Can't explicitly copy regions");
      }

      public StemMemberKind StemMemberKind
      { get { return StemMemberKind.RegionEnd; } }

      public MemberKind MemberKind
      { get { return MemberKind.RegionEnd; } }

      public IBlockStartDetail BlockStart { get; private set; }
   }
}
