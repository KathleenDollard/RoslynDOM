using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

// TODO: Combine this with Method and constructor
namespace RoslynDom
{
   public class RDomOperator : RDomBase<IOperator, IMethodSymbol>, IOperator
   {
      private RDomCollection<IParameter> _parameters;
      private RDomCollection<IStatementCommentWhite> _statements;
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomOperator(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomOperator(RDomOperator oldRDom)
           : base(oldRDom)
      {
         Initialize();
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
         Parameters.AddOrMoveRange(newParameters);
         var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
         StatementsAll.AddOrMoveRange(newStatements);
         IsStatic = oldRDom.IsStatic;
         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         Operator = oldRDom.Operator;
         Type = oldRDom.Type.Copy();
        }

      private void Initialize()
      {
         _statements = new RDomCollection<IStatementCommentWhite>(this);
         _parameters = new RDomCollection<IParameter>(this);
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

      public string Name { get; set; }
      public IReferencedType Type { get; set; }
      public AccessModifier AccessModifier { get; set; }
      public AccessModifier DeclaredAccessModifier { get; set; }
      public Operator Operator { get; set; }
      public bool IsStatic { get; set; }
      public IStructuredDocumentation StructuredDocumentation { get; set; }
      public string Description { get; set; }

      public RDomCollection<IParameter> Parameters
      { get { return _parameters; } }

      public RDomCollection<IStatementCommentWhite> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      public bool HasBlock
      {
         get { return true; }
         set { }
      }

      public MemberKind MemberKind
      { get { return MemberKind.Operator; } }

   }
}
