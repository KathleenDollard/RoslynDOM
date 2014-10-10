using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomInvalidMember : RDomBase<IInvalidMember, ISymbol>, IInvalidMember
   {
      private AttributeCollection _attributes = new AttributeCollection();
      public RDomInvalidMember(string name, AccessModifier accessModifier = AccessModifier.Private)
            : this(null, null, null)
      {
         NeedsFormatting = true;
         Name = name;
         AccessModifier = accessModifier;
      }

      public RDomInvalidMember(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomInvalidMember(RDomInvalidMember oldRDom)
           : base(oldRDom)
      {
         Initialize();
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "Follows general pattern, but is currently unused, may later remove")]
      private void Initialize()
      { }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
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
      {
         get { return MemberKind.Invalid; }
      }

      public StemMemberKind StemMemberKind
      {
         get { return StemMemberKind.Invalid; }
      }
   }
}
