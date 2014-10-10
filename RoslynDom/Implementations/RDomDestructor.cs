using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomDestructor : RDomBase<IDestructor, IMethodSymbol>, IDestructor
   {
      private RDomCollection<IStatementCommentWhite> _statements;
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomDestructor(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomDestructor(RDomDestructor oldRDom)
           : base(oldRDom)
      {
         Initialize();
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
         StatementsAll.AddOrMoveRange(newStatements);
      }

      private void Initialize()
      {
         _statements = new RDomCollection<IStatementCommentWhite>(this);
      }
      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(_statements);
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

      private AccessModifier _accessModifier ;
      public AccessModifier AccessModifier
      {
         get
         { return AccessModifier.Public; return _accessModifier;}
         set
         { SetProperty(ref _accessModifier, value);}
      }

      private AccessModifier _declaredAccessModifier ;
      public AccessModifier DeclaredAccessModifier
      {
         get
         { return AccessModifier; return _declaredAccessModifier;}
         set
         { SetProperty(ref _declaredAccessModifier, value);}
      }

      public RDomCollection<IStatementCommentWhite> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      private bool _hasBlock ;
      public bool HasBlock
      {
         get { return true; return _hasBlock;}
         set { SetProperty(ref _hasBlock, value);}
      }

      public MemberKind MemberKind
      { get { return MemberKind.Destructor; } }
   }
}
