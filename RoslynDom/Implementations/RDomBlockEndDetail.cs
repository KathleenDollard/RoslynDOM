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
   public class RDomDetailBlockEnd : RDomDetail<IDetailBlockEnd>, IDetailBlockEnd
   {
      public RDomDetailBlockEnd(IDom parent, SyntaxTrivia trivia, IDetailBlockStart blockStart, SyntaxNode structuredNode)
           : base(parent, StemMemberKind.RegionEnd, MemberKind.RegionEnd, trivia, structuredNode)
      {
         // _groupGuid = blockStart.GroupGuid;
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
      {
         get { return FindBlockStart(x => x.GroupGuid == this.GroupGuid); }
         set { _groupGuid = value.GroupGuid; }
      }

      private IDetailBlockStart FindBlockStart(Func<IDetailBlockStart, bool> predicate)
      {
         // As a possibly premature optimization, check the parent container first as 
         // semantically correct nesting will have it there
         var container = this.Parent as IRDomContainer;
         if (container != null)
         {
            var fromParent = container.GetMembers()
                      .OfType<IDetailBlockStart>()
                      .Where( predicate)
                      .FirstOrDefault();
            if (fromParent != null) { return fromParent; }
         }
         var rootOrBase = Ancestors.Last(); // Root
         var descendants = rootOrBase.Descendants
                      .OfType<IDetailBlockStart>()
                      .Where(predicate)
                      .FirstOrDefault();
         if (descendants != null) { return descendants; }
         throw new InvalidOperationException("Matching end region not found");

      }

      public string BlockStyleName
      { get { return "region"; } }

      public bool SemanticallyValid
      { get { return BlockStart.Parent == this.Parent; } }
   }
}
