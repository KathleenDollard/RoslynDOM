using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;
using cm = System.ComponentModel;

namespace RoslynDom
{
   public class RDomEvent : RDomBase<IEvent, IEventSymbol>, IEvent
   {
      private AttributeCollection _attributes = new AttributeCollection();

      /// <summary>
      /// Constructor to use when creating a RoslynDom from scratch
      /// </summary>
      /// <param name="name">
      /// Name of the event
      /// </param>
      /// <param name="typeName">
      /// Type name of the event</param>
      /// <param name="accessModifier">
      /// The accessibilty (scope) modifier. Default is the most restrictive scope, private.
      /// </param>
      /// <param name="isAbstract">
      /// Pass true for an abstract class
      /// </param>
      /// <param name="isVirtual">
      /// Pass true for an virtual class
      /// </param>
      /// <param name="isOverride">
      /// Pass true for an override class
      /// </param>
      /// <param name="isSealed">
      /// Pass true for an sealed class
      /// </param>
      /// <param name="isStatic">
      /// Pass true for an static class
      /// </param>
      /// <param name="isNew">
      /// Pass true for an new class
      /// </param>
      public RDomEvent(string name, string typeName, AccessModifier accessModifier=AccessModifier.Private,
               bool isAbstract = false, bool isVirtual = false, bool isOverride = false, 
               bool isSealed = false, bool isStatic = false, bool isNew = false)
       : this(name, new RDomReferencedType(typeName), accessModifier, isAbstract,
                isVirtual, isOverride,  isSealed, isStatic, isNew )
      {      }

      /// <summary>
      /// Constructor to use when creating a RoslynDom from scratch
      /// </summary>
      /// <param name="name">
      /// Name of the event
      /// </param>
      /// <param name="typeName">
      /// Type name of the event</param>
      /// <param name="accessModifier">
      /// The accessibilty (scope) modifier. Default is the most restrictive scope, private.
      /// </param>
      /// <param name="isAbstract">
      /// Pass true for an abstract class
      /// </param>
      /// <param name="isVirtual">
      /// Pass true for an virtual class
      /// </param>
      /// <param name="isOverride">
      /// Pass true for an override class
      /// </param>
      /// <param name="isSealed">
      /// Pass true for an sealed class
      /// </param>
      /// <param name="isStatic">
      /// Pass true for an static class
      /// </param>
      /// <param name="isNew">
      /// Pass true for an new class
      /// </param>
      public RDomEvent(string name, IReferencedType type, AccessModifier accessModifier = AccessModifier.Private,
               bool isAbstract = false, bool isVirtual = false, bool isOverride = false,
               bool isSealed = false, bool isStatic = false, bool isNew = false)
       : this(null, null, null)
      {
         Name = name;
         Type = type;
         AccessModifier = accessModifier;
         IsAbstract = isAbstract;
         IsVirtual = isVirtual;
         IsOverride = isOverride;
         IsSealed = isSealed;
         IsStatic = isStatic;
         IsNew = isNew;
      }

      [cm.EditorBrowsable(cm.EditorBrowsableState.Never)]
      public RDomEvent(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

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

      public AttributeCollection Attributes
      { get { return _attributes; } }

      public string Name { get; set; }
      public IReferencedType Type { get; set; }
      public AccessModifier AccessModifier { get; set; }
      public AccessModifier DeclaredAccessModifier { get; set; }
      public bool IsAbstract { get; set; }
      public bool IsVirtual { get; set; }
      public bool IsOverride { get; set; }
      public bool IsSealed { get; set; }
      public bool IsNew { get; set; }
      public bool IsStatic { get; set; }
      public IStructuredDocumentation StructuredDocumentation { get; set; }
      public string Description { get; set; }

      public MemberKind MemberKind
      { get { return MemberKind.Event; } }

   }
}
