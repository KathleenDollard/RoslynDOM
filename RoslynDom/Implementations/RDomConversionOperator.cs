using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom
{
   public class RDomConversionOperator : RDomBase<IConversionOperator, IMethodSymbol>, IConversionOperator
   {
      private RDomCollection<IParameter> _parameters;
      private RDomCollection<IStatementCommentWhite> _statements;
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomConversionOperator(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
     "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomConversionOperator(RDomConversionOperator oldRDom)
           : base(oldRDom)
      {
         Initialize();
         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         IsImplicit = oldRDom.IsImplicit;
         IsStatic = oldRDom.IsStatic;
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
         Parameters.AddOrMoveRange(newParameters);
         var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
         StatementsAll.AddOrMoveRange(newStatements);

         Type = oldRDom.Type.Copy();
      }

      protected void Initialize()
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

      //public override IEnumerable<IDom> Descendants
      //{
      //    get
      //    {
      //        var list = base.Descendants.ToList();
      //        foreach (var statement in _statements)
      //        { list.AddRange(statement.DescendantsAndSelf); }
      //        return list;
      //    }
      //}

      public string Name { get; set; }

      //public string OuterName
      //{ get { return RoslynUtilities.GetOuterName(this); } }


      public AttributeCollection Attributes
      { get { return _attributes; } }

      public AccessModifier AccessModifier { get; set; }

      public AccessModifier DeclaredAccessModifier { get; set; }

      public IReferencedType Type { get; set; }
      public bool IsImplicit { get; set; }
      public bool IsStatic { get; set; }
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
      { get { return MemberKind.ConversionOperator; } }

      public IStructuredDocumentation StructuredDocumentation { get; set; }

      public string Description { get; set; }
   }
}
