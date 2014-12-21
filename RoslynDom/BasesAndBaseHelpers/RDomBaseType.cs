using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public abstract class RDomBaseType<T>
       : RDomBase<T, INamedTypeSymbol>, IType<T>, ITypeMemberContainer
       where T : class, IType<T>
   {
      private RDomCollection<ITypeMemberAndDetail> _members;
      private RDomCollection<IReferencedType> _implementedInterfaces;
      private IEnumerable<IReferencedType> _allImplementedInterfaces;
      private MemberKind _memberKind;
      private StemMemberKind _stemMemberKind;
      private RDomCollection<ITypeParameter> _typeParameters;
      private AttributeCollection _attributes = new AttributeCollection();

      protected RDomBaseType(string metadataName, AccessModifier declaredAccessModifier,
            MemberKind memberKind,
            StemMemberKind stemMemberKind)
         : base()
      {
         Initialize();
         _memberKind = memberKind;
         _stemMemberKind = stemMemberKind;
         _metadataName = metadataName;
         DeclaredAccessModifier = declaredAccessModifier; // Must use the setter here!
         if (metadataName.Contains("."))
         {
            _name = metadataName.SubstringAfterLast(".");
         }
         else
         {
            _name = metadataName;
         }
      }

      internal RDomBaseType(
            SyntaxNode rawItem,
            IDom parent,
            SemanticModel model,
            MemberKind memberKind,
            StemMemberKind stemMemberKind)
         : base(rawItem, parent, model)
      {
         Initialize();
         _memberKind = memberKind;
         _stemMemberKind = stemMemberKind;
         _name = TypedSymbol.Name;
         _metadataName = TypedSymbol.ContainingNamespace + "." + TypedSymbol.MetadataName;

      }

      internal RDomBaseType(T oldIDom)
           : base(oldIDom)
      {
         Initialize();
         var oldRDom = oldIDom as RDomBaseType<T>;
         _name = oldRDom.Name;
         _metadataName = oldRDom.MetadataName;
         _accessModifier = oldRDom.AccessModifier;
         _declaredAccessModifier = oldRDom.DeclaredAccessModifier;
         _memberKind = oldRDom._memberKind;
         _stemMemberKind = oldRDom._stemMemberKind;
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         RDomCollection<ITypeMemberAndDetail>.Copy(oldRDom.MembersAll, _members);
         _typeParameters = oldRDom.TypeParameters.Copy(this);

         // TODO: _allImplementedInterfaces = oldRDom._allImplementedInterfaces.Select(x => x.Copy());
         _implementedInterfaces.AddOrMoveRange(oldRDom._implementedInterfaces.Select(x => x.Copy()));
      }

      private void Initialize()
      {
         _members = new RDomCollection<ITypeMemberAndDetail>(this);
         _typeParameters = new RDomCollection<ITypeParameter>(this);
         _implementedInterfaces = new RDomCollection<IReferencedType>(this);

         var typeSymbol = Symbol as ITypeSymbol;
         // TODO: _allImplementedInterfaces = typeSymbol.AllInterfaces
         //if (typeSymbol == null) throw new NotImplementedException();
      }

      public bool AddOrMoveMember(IDom item)
      { return _members.AddOrMove(item); }

      public bool RemoveMember(IDom item)
      { return _members.Remove(item); }

      public bool InsertOrMoveMember(int index, IDom item)
      { return _members.InsertOrMove(index, item); }

      public IEnumerable<IDom> GetMembers()
      { return MembersAll.ToList(); }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(_members);
            return list;
         }
      }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      private string _metadataName;
      [Required]
      public string MetadataName
      {
         get { return _metadataName; }
         set { SetProperty(ref _metadataName, value); }
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

      public string OuterName
      { get { return RoslynUtilities.GetOuterName(this); } }

      public string QualifiedName
      { get { return RoslynUtilities.GetQualifiedName(this); } }

      public string Namespace
      { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

      public IType ContainingType
      { get { return Parent as IType; } }

      public bool IsNested
      { get { return (Parent is IType); } }

      public RDomCollection<ITypeParameter> TypeParameters
      { get { return _typeParameters; } }

      public RDomCollection<ITypeMemberAndDetail> MembersAll
      { get { return _members; } }

      public IEnumerable<ITypeMember> Members
      { get { return _members.OfType<ITypeMember>().ToList(); } }

      public IEnumerable<IMethod> Methods
      { get { return Members.OfType<IMethod>().ToList(); } }

      public IEnumerable<IProperty> Properties
      { get { return Members.OfType<IProperty>().ToList(); } }

      public IEnumerable<IEvent> Events
      { get { return Members.OfType<IEvent>().ToList(); } }

      // This is not yet editable because it is non-trivial to ensure 
      // correct interface usage (appearing once, etc). These semantics
      // may also change as "all" is confusing with other use in RoslynDon
      public RDomCollection<IReferencedType> ImplementedInterfaces
      { get { return _implementedInterfaces; } }

      public IEnumerable<IReferencedType> AllImplementedInterfaces
      {
         get
         {
            // TODO: Figure out all interfaces or don't support
            return null;
         }
      }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      public MemberKind MemberKind
      { get { return _memberKind; } }

      public StemMemberKind StemMemberKind
      { get { return _stemMemberKind; } }

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
   }
}