using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public abstract class RDomBaseType<T>
       : RDomBase<T, INamedTypeSymbol>, IType<T>, ITypeMemberContainer
       where T : class, IType<T>
   {
      private RDomCollection<ITypeMemberCommentWhite> _members;
      private RDomCollection<IReferencedType> _implementedInterfaces;
      private IEnumerable<IReferencedType> _allImplementedInterfaces;
      private MemberKind _memberKind;
      private StemMemberKind _stemMemberKind;
      private RDomCollection<ITypeParameter> _typeParameters;
      private AttributeCollection _attributes = new AttributeCollection();

      internal RDomBaseType(
            SyntaxNode rawItem,
            IDom parent,
            SemanticModel model,
            MemberKind memberKind,
            StemMemberKind stemMemberKind)
         : base(rawItem, parent, model)
      {
         _memberKind = memberKind;
         _stemMemberKind = stemMemberKind;
         Initialize();
      }


      internal RDomBaseType(T oldIDom)
           : base(oldIDom)
      {
         Initialize();
         var oldRDom = oldIDom as RDomBaseType<T>;
         _memberKind = oldRDom._memberKind;
         _stemMemberKind = oldRDom._stemMemberKind;
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         MembersAll.AddOrMoveRange(RoslynDomUtilities.CopyMembers(oldRDom._members));
         TypeParameters.AddOrMoveRange(RoslynDomUtilities.CopyMembers(oldRDom._typeParameters));

         // TODO: _allImplementedInterfaces = oldRDom._allImplementedInterfaces.Select(x => x.Copy());
         _implementedInterfaces.AddOrMoveRange(oldRDom._implementedInterfaces.Select(x => x.Copy()));
      }

      private void Initialize()
      {
         _members = new RDomCollection<ITypeMemberCommentWhite>(this);
         _typeParameters = new RDomCollection<ITypeParameter>(this);
         _implementedInterfaces = new RDomCollection<IReferencedType>(this);

         var typeSymbol = Symbol as ITypeSymbol;
         // TODO: _allImplementedInterfaces = typeSymbol.AllInterfaces
         if (typeSymbol == null) throw new NotImplementedException();
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(_members);
            return list;
         }
      }

      public string Name { get; set; }
      public AccessModifier AccessModifier { get; set; }
      public AccessModifier DeclaredAccessModifier { get; set; }

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

      public RDomCollection<ITypeMemberCommentWhite> MembersAll
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

      public IStructuredDocumentation StructuredDocumentation { get; set; }

      public string Description { get; set; }
   }
}