using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public class RDomProperty : RDomBase<IProperty, IPropertySymbol>, IProperty
   {
      private RDomCollection<IParameter> _parameters;
      private AttributeCollection _attributes = new AttributeCollection();
      // The RDomList is used for accessor to reuse the forced parenting in that class
      private RDomCollection<IAccessor> _accessors;

      public RDomProperty(string name, string propertyTypeName, AccessModifier declaredAccessModifier = AccessModifier.Private , 
                  bool isAbstract=false, bool isVirtual = false, bool isOverride = false, bool isSealed = false, bool isStatic = false, 
                  bool isNew = false, bool isWriteOnly = false, bool isReadOnly = false)
         : this(name, declaredAccessModifier, isAbstract, isVirtual, isOverride, isSealed, 
                  isStatic, isNew, isWriteOnly, isReadOnly)
      {
         PropertyType = new RDomReferencedType(this, propertyTypeName);
      }

      public RDomProperty( string name, IReferencedType propertyType, AccessModifier declaredAccessModifier = AccessModifier.Private,
                  bool isAbstract = false, bool isVirtual = false, bool isOverride = false, bool isSealed = false, bool isStatic = false,
                  bool isNew = false, bool isWriteOnly = false, bool isReadOnly = false)
         : this( name, declaredAccessModifier, isAbstract, isVirtual, isOverride, isSealed,
                  isStatic, isNew, isWriteOnly, isReadOnly)
      {
         PropertyType = propertyType;
      }

      private RDomProperty( string name,  AccessModifier declaredAccessModifier = AccessModifier.Private,
                 bool isAbstract = false, bool isVirtual = false, bool isOverride = false, bool isSealed = false, bool isStatic = false,
                 bool isNew = false, bool isWriteOnly = false, bool isReadOnly = false)
            : base()
      {
         Initialize();
         Name = name;
         DeclaredAccessModifier = declaredAccessModifier; // Must use the setter here!
         IsAbstract = isAbstract;
         IsVirtual = isVirtual;
         IsOverride = isOverride;
         IsSealed = isSealed;
         IsStatic = isStatic;
         IsNew = isNew;
         if (!isReadOnly)
         {
            var accesor = new RDomPropertyAccessor(this,  AccessorType.Set);
            SetAccessor = accesor;
         }
         if (!isWriteOnly)
         {
            var accesor = new RDomPropertyAccessor(this,  AccessorType.Get);
            GetAccessor = accesor;
         }

      }

      public RDomProperty(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomProperty(RDomProperty oldRDom)
          : base(oldRDom)
      {
         Initialize();
         _parameters = oldRDom.Parameters.Copy(this);
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         GetAccessor = oldRDom.GetAccessor == null ? null : oldRDom.GetAccessor.Copy();
         SetAccessor = oldRDom.SetAccessor == null ? null : oldRDom.SetAccessor.Copy();
         _name = oldRDom.Name;
         _propertyType = oldRDom.PropertyType.Copy();
         _accessModifier = oldRDom.AccessModifier;
         _declaredAccessModifier = oldRDom.DeclaredAccessModifier;
         _isAbstract = oldRDom.IsAbstract;
         _isVirtual = oldRDom.IsVirtual;
         _isOverride = oldRDom.IsOverride;
         _isSealed = oldRDom.IsSealed;
         _isStatic = oldRDom.IsStatic;
         _isNew = oldRDom.IsNew;
      }

      private void Initialize()
      {
         _parameters = new RDomCollection<IParameter>(this);
         _accessors = new RDomCollection<IAccessor>(this);
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            if (GetAccessor != null)
            { list.Add(GetAccessor); }
            if (SetAccessor != null)
            { list.Add(SetAccessor); }
            return list;
         }
      }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      public bool CanBeAutoProperty
      {
         get
         {
            return !(_accessors
               .Where(x => x.Statements.Any())
               .Any());
         }
      }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      private IReferencedType _propertyType;
      [Required]
      public IReferencedType PropertyType
      {
         get { return _propertyType; }
         set { SetProperty(ref _propertyType, value); }
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
         set
         {
            SetProperty(ref _declaredAccessModifier, value);
            AccessModifier = value;
         }
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

      private bool _isStatic;
      public bool IsStatic
      {
         get { return _isStatic; }
         set { SetProperty(ref _isStatic, value); }
      }

      private bool _isNew;
      public bool IsNew
      {
         get { return _isNew; }
         set { SetProperty(ref _isNew, value); }
      }

      public bool CanGet
      {
         get { return GetAccessor != null; }
      }

      public bool CanSet
      {
         get { return SetAccessor != null; }
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

      public IAccessor GetAccessor
      {
         get
         {
            return _accessors
                  .Where(x => x.AccessorType == AccessorType.Get)
                  .FirstOrDefault();
         }
         set
         {
            if (value == null) return;
            _accessors.Remove(GetAccessor);
            _accessors.AddOrMove(value);
         }
      }

      public IAccessor SetAccessor
      {
         get
         {
            return _accessors
                  .Where(x => x.AccessorType == AccessorType.Set)
                  .FirstOrDefault();
         }
         set
         {
            if (value == null) return;
            _accessors.Remove(SetAccessor);
            _accessors.AddOrMove(value);
         }
      }

      /// <summary></summary>
      /// <returns></returns>
      /// <remarks>
      /// This is to support VB, C# does not have parameters on properties. Property parameters
      /// in VB are generally used for indexing, which is managed by "default" in C#
      /// <br />
      /// Can't test until VB is active
      /// This is for VB, wihch I have not yet implemented, but don't want things crashing so will ignore
      /// </remarks>
      public RDomCollection<IParameter> Parameters
      { get { return _parameters; } }

      public MemberKind MemberKind
      { get { return MemberKind.Property; } }

      IReferencedType IHasReturnType.ReturnType
      {
         get { return PropertyType; }
         set { PropertyType = value; }
      }
   }
}
