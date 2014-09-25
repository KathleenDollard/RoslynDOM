using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomProperty : RDomBase<IProperty, IPropertySymbol>, IProperty
   {
      private RDomCollection<IParameter> _parameters;
      private AttributeCollection _attributes = new AttributeCollection();
      // The RDomList is used for accessor to reuse the forced parenting in that class
      private RDomCollection<IAccessor> _accessors; 


      public RDomProperty(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomProperty(RDomProperty oldRDom)
          : base(oldRDom)
      {
         Initialize();
         var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
         Parameters.AddOrMoveRange(newParameters);
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         GetAccessor = oldRDom.GetAccessor == null ? null : oldRDom.GetAccessor.Copy();
         SetAccessor = oldRDom.SetAccessor == null ? null : oldRDom.SetAccessor.Copy();
         ReturnType = oldRDom.ReturnType;
         IsAbstract = oldRDom.IsAbstract;
         IsVirtual = oldRDom.IsVirtual;
         IsOverride = oldRDom.IsOverride;
         IsSealed = oldRDom.IsSealed;
         IsStatic = oldRDom.IsStatic;
         IsNew = oldRDom.IsNew;
         CanGet = oldRDom.CanGet;
         CanSet = oldRDom.CanSet;
      }

      protected void Initialize()
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

      public string Name { get; set; }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      public bool CanBeAutoProperty
      {
         get {
            return !(_accessors
               .Where(x => x.Statements.Any())
               .Any());
         }
      }

      public IReferencedType PropertyType { get; set; }

      public AccessModifier AccessModifier { get; set; }
      public AccessModifier DeclaredAccessModifier { get; set; }
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

      public IReferencedType ReturnType
      {
         get { return PropertyType; }
         set { PropertyType = value; }
      }

      public bool IsAbstract { get; set; }

      public bool IsVirtual { get; set; }

      public bool IsOverride { get; set; }

      public bool IsSealed { get; set; }

      public bool IsStatic { get; set; }

      public bool CanGet { get; set; }

      public bool CanSet { get; set; }

      public bool IsNew { get; set; }

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

      public override object RequestValue(string propertyName)
      {
         if (propertyName == "TypeName")
         { return ReturnType.QualifiedName; }
         return base.RequestValue(propertyName);
      }
      public IStructuredDocumentation StructuredDocumentation { get; set; }

      public string Description { get; set; }
   }
}
