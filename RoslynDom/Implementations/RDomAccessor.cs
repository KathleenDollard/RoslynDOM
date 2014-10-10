using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomPropertyAccessor : RDomBase<IAccessor, ISymbol>, IAccessor
   {
      private RDomCollection<IStatementCommentWhite> _statements;
      private AttributeCollection _attributes = new AttributeCollection();
      private AccessorType _accessorType;

      public RDomPropertyAccessor(string name, AccessorType accessorType, AccessModifier accessModifier = AccessModifier.Private )
      : this(null, AccessorType.Unknown , null, null)
      {
         NeedsFormatting = true;
         Name = name;
         AccessModifier = accessModifier;
      }

      public RDomPropertyAccessor(SyntaxNode rawItem, AccessorType accessorType, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
         _accessorType = accessorType;
         Initialize();
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomPropertyAccessor(RDomPropertyAccessor oldRDom)
              : base(oldRDom)
      {
         Initialize();
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
         StatementsAll.AddOrMoveRange(newStatements);

         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         HasBlock = oldRDom.HasBlock;
         _accessorType = oldRDom.AccessorType;
      }

      protected void Initialize()
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

      public RDomCollection<IStatementCommentWhite> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      public AccessorType AccessorType
      { get { return _accessorType; } }

      private bool _hasBlock;
      public bool HasBlock
      {
         get { return true; return _hasBlock; }
         set { SetProperty(ref _hasBlock, value); }
      }
   }
}
