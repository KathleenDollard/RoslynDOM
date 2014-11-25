using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System;

namespace RoslynDom
{
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
      "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
      Justification = "Because this represents an enum, it's an appropriate name")]
   public class RDomEnum : RDomBase<IEnum, ISymbol>, IEnum
   {
      private AttributeCollection _attributes = new AttributeCollection();
      private RDomCollection<IEnumMember> _members;

      public RDomEnum( string name, string metadataName, string underlyingTypeName = null, AccessModifier accessModifier = AccessModifier.Private)
          : this(name, metadataName,accessModifier)
      {
         _underlyingType = new RDomReferencedType(this, underlyingTypeName, true);
      }

      public RDomEnum( string name, string metadataName, IReferencedType underlyingType, AccessModifier accessModifier = AccessModifier.Private)
          : this( name, metadataName,  accessModifier)
      {
         _underlyingType = underlyingType;
      }

      private RDomEnum( string name, string metadataName, AccessModifier accessModifier = AccessModifier.Private)
          : this((SyntaxNode)null, null, null)
      {
         _name = name;
         _metadataName = metadataName;
         _accessModifier = accessModifier;
      }

      public RDomEnum(SyntaxNode rawItem, IDom parent, SemanticModel model)
          : base(rawItem, parent, model)
      {
         Initialize();
         Name = TypedSymbol.Name;
         MetadataName = TypedSymbol.ContainingNamespace + "." + TypedSymbol.MetadataName;
      }

      internal RDomEnum(RDomEnum oldRDom)
          : base(oldRDom)
      {
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         _members = oldRDom.Members.Copy(this);
         _name = oldRDom.Name;
         _metadataName = oldRDom.MetadataName;
         _accessModifier = oldRDom.AccessModifier;
         _declaredAccessModifier = oldRDom.DeclaredAccessModifier;
         _underlyingType = oldRDom.UnderlyingType == null ? null : oldRDom.UnderlyingType.Copy();
      }

      private void Initialize()
      {
         _members = new RDomCollection<IEnumMember>(this);
      }

      public IEnumerable<IDom> GetMembers()
      { return Members; }

      public bool AddOrMoveMember(IDom item)
      { return Members.AddOrMove(item); }

      public bool RemoveMember(IDom item)
      { return Members.Remove(item); }

      public bool InsertOrMoveMember(int index, IDom item)
      { return Members.InsertOrMove(index, item); }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(_members);
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
         set { SetProperty(ref _declaredAccessModifier, value); }
      }

      private IReferencedType _underlyingType;
      public IReferencedType UnderlyingType
      {
         get { return _underlyingType; }
         set { SetProperty(ref _underlyingType, value); }
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

      public string OuterName
      { get { return RoslynUtilities.GetOuterName(this); } }

      public string QualifiedName
      { get { return RoslynUtilities.GetQualifiedName(this); } }

      public string Namespace
      { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

      public bool IsNested
      { get { return (Parent is IType); } }

      public IType ContainingType
      { get { return Parent as IType; } }

      public RDomCollection<IEnumMember> Members
      { get { return _members; } }

      public MemberKind MemberKind
      { get { return MemberKind.Enum; } }

      public StemMemberKind StemMemberKind
      { get { return StemMemberKind.Enum; } }
   }
}
