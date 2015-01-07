using Microsoft.CodeAnalysis;
using RoslynDom.Common;
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
   public class RDomDetailBlockEnd : RDomDetail<IDetailBlockEnd>, IDetailBlockEnd
   {
      public RDomDetailBlockEnd(IDom parent, SyntaxTrivia trivia, IDetailBlockStart blockStart)
         : base(parent, StemMemberKind.RegionEnd, MemberKind.RegionEnd, trivia)
      {
         _groupGuid = blockStart.GroupGuid;
      }

      internal RDomDetailBlockEnd(RDomDetailBlockEnd oldRDom)
         : base(oldRDom)
      {
         // Group id is set with the parent because parent is needed to set
         // The assumption is that copied groups must be immediately placed in a container to find each other.
         _oldGroupGuid = oldRDom.GroupGuid;
      }

      public override IDom Parent
      {
         get { return base.Parent; }
         set
         {
            base.Parent = value;
            if (_oldGroupGuid != Guid.Empty)
            {
               var start = FindBlockStart(x => ((RDomDetailBlockStart)x).OldGroupGuid == _oldGroupGuid);
               _groupGuid = start.GroupGuid;
               _oldGroupGuid = Guid.Empty;
               ((RDomDetailBlockStart)start).OldGroupGuid = Guid.Empty;
            }
         }
      }

      private Guid _oldGroupGuid;

      internal Guid OldGroupGuid { get { return _oldGroupGuid; } }

      private Guid _groupGuid;

      public Guid GroupGuid { get { return _groupGuid; } }

      public IDetailBlockStart BlockStart
      { get { return FindBlockStart(x => x.GroupGuid == this.GroupGuid); } }

      private IDetailBlockStart FindBlockStart(Func<IDetailBlockStart, bool> predicate)
      {
         var parentContainers = Ancestors.OfType<IContainer>();
         foreach (var container in parentContainers)
         {
            // TODO: I'm pretty sure you just need predicate, not the extra lambda, but want to complete testing before I check.
            var ret = container.GetMembers()
                      .OfType<IDetailBlockStart>()
                      .Where(x => predicate(x))
                      .SingleOrDefault();
            if (ret != null) { return ret; }
         }
         throw new InvalidOperationException("Matching start region not found");
      }

      public string BlockStyleName
      { get { return "region"; } }
   }
}