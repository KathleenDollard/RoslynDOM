using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System;

namespace RoslynDom
{
   public class RDomRootGroup : RDomBase<IRootGroup>, IRootGroup
   {
      private Compilation _compilation;
      private RDomCollection<IRoot> _roots;

      public RDomRootGroup(Compilation compilation)
      {
         _compilation = compilation;
         Initialize();
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomRootGroup(RDomRootGroup oldRDom)
      {
         _roots = Roots.Copy(this);
      }

      private void Initialize()
      {
         _roots = new RDomCollection<IRoot>(this);
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(_roots);
            return list;
         }
      }

      public bool HasSyntaxErrors
      { get { return Roots.Any(x => x.HasSyntaxErrors); } }

      public override object OriginalRawItem
      { get { return _compilation; } }

      public override object RawItem
      { get { return OriginalRawItem; } }

      public RDomCollection<IRoot> Roots
      { get { return _roots; } }

      public override ISymbol Symbol
      { get { return null; } }
   }
}
