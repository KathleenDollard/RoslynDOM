using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;
using cm=System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
      public RDomEvent(string name, string typeName, AccessModifier accessModifier = AccessModifier.Private,
               bool isAbstract = false, bool isVirtual = false, bool isOverride = false,
               bool isSealed = false, bool isStatic = false, bool isNew = false)
       : this(name, new RDomReferencedType(typeName), accessModifier, isAbstract,
                isVirtual, isOverride, isSealed, isStatic, isNew)
      { }

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
         _name = name;
         _type = type;
         _accessModifier = accessModifier;
         _isAbstract = isAbstract;
         _isVirtual = isVirtual;
         _isOverride = isOverride;
         _isSealed = isSealed;
         _isStatic = isStatic;
         _isNew = isNew;
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

         _name = oldRDom.Name;
         _type = oldRDom.Type.Copy();
         _accessModifier = oldRDom.AccessModifier;
         _declaredAccessModifier = oldRDom.DeclaredAccessModifier;
         _isAbstract = oldRDom.IsAbstract;
         _isVirtual = oldRDom.IsVirtual;
         _isOverride = oldRDom.IsOverride;
         _isSealed = oldRDom.IsSealed;
         _isStatic = oldRDom.IsStatic;
         _isNew = oldRDom.IsNew;
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

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }
      private IReferencedType _type;
      [Required]
      public IReferencedType Type
      {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }

      private AccessModifier _accessModifier;
      public AccessModifier AccessModifier
      {
         get { return _accessModifier; }
         set { SetProperty(ref _accessModifier, value); }
      }

      private AccessModifier _declaredAccessModifier;
      public AccessModifier DeclaredAccessModifier
      {
         get { return _declaredAccessModifier; }
         set { SetProperty(ref _declaredAccessModifier, value); }
      }

      private bool _isAbstract;
      public bool IsAbstract
      {
         get { return _isAbstract; }
         set { SetProperty(ref _isAbstract, value); }
      }

      private bool _isVirtual;
      public bool IsVirtual
      {
         get { return _isVirtual; }
         set { SetProperty(ref _isVirtual, value); }
      }

      private bool _isOverride;
      public bool IsOverride
      {
         get { return _isOverride; }
         set { SetProperty(ref _isOverride, value); }
      }

      private bool _isSealed;
      public bool IsSealed
      {
         get { return _isSealed; }
         set { SetProperty(ref _isSealed, value); }
      }

      private bool _isNew;
      public bool IsNew
      {
         get { return _isNew; }
         set { SetProperty(ref _isNew, value); }
      }

      private bool _isStatic;
      public bool IsStatic
      {
         get { return _isStatic; }
         set { SetProperty(ref _isStatic, value); }
      }

      private IStructuredDocumentation _structuredDocumentation;
      public IStructuredDocumentation StructuredDocumentation
      {
         get { return _structuredDocumentation; }
         set { SetProperty(ref _structuredDocumentation, value); }
      }

      private string _description;
      public string Description
      {
         get { return _description; }
         set { SetProperty(ref _description, value); }
      }

      public MemberKind MemberKind
      { get { return MemberKind.Event; } }
   }
}
