using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom
{
   public class RDomEvent : RDomBase<IEvent, IEventSymbol>, IEvent
   {
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomEvent(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {  }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
     "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomEvent(RDomEvent oldRDom)
           : base(oldRDom)
      {
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));

         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         Type = oldRDom.Type;
         IsAbstract = oldRDom.IsAbstract;
         IsVirtual = oldRDom.IsVirtual;
         IsOverride = oldRDom.IsOverride;
         IsSealed = oldRDom.IsSealed;
         IsStatic = oldRDom.IsStatic;
         IsNew = oldRDom.IsNew;
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            return list;
         }
      }

      public string Name { get; set; }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      public AccessModifier AccessModifier { get; set; }
      public AccessModifier DeclaredAccessModifier { get; set; }
      public IReferencedType Type { get; set; }
      public bool IsAbstract { get; set; }
      public bool IsVirtual { get; set; }
      public bool IsOverride { get; set; }
      public bool IsSealed { get; set; }
      public bool IsStatic { get; set; }
      public bool IsNew { get; set; }

      public MemberKind MemberKind
      { get { return MemberKind.Event; } }

      public IStructuredDocumentation StructuredDocumentation { get; set; }

      public string Description { get; set; }
   }
}
