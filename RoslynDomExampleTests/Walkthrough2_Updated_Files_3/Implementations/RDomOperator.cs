using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
// TODO: Combine this with Method and constructor
namespace RoslynDom
{
   public class RDomOperator : RDomBase<IOperator, IMethodSymbol>, IOperator
   {
      private RDomCollection<IParameter> _parameters;
      private RDomCollection<IStatementCommentWhite> _statements;
      private AttributeCollection _attributes = new AttributeCollection();
 public RDomOperator (string               _name,string _typeName,AccessModifier               _accessModifier,AccessModifier               _declaredAccessModifier,Operator               _operator,bool               _isStatic,IStructuredDocumentation               _structuredDocumentation,string               _description,bool               _hasBlock)
 : this (_name,new RDomReferencedType(_typeName),_accessModifier,_declaredAccessModifier,_operator,_isStatic,_structuredDocumentation,_description,_hasBlock ) { NeedsFormatting=true;
 Name=_name;
 Type=_type;
 AccessModifier=_accessModifier;
 DeclaredAccessModifier=_declaredAccessModifier;
 Operator=_operator;
 IsStatic=_isStatic;
 StructuredDocumentation=_structuredDocumentation;
 Description=_description;
 HasBlock=_hasBlock;
 }public RDomOperator(string _name, IReferencedType _type, AccessModifier _accessModifier, AccessModifier _declaredAccessModifier, Operator _operator, bool _isStatic, IStructuredDocumentation _structuredDocumentation, string _description, bool _hasBlock)
: this(null, null, null)
{
    NeedsFormatting = true;
    Name = _name;
    Type = _type;
    AccessModifier = _accessModifier;
    DeclaredAccessModifier = _declaredAccessModifier;
    Operator = _operator;
    IsStatic = _isStatic;
    StructuredDocumentation = _structuredDocumentation;
    Description = _description;
    HasBlock = _hasBlock;
}
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

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }
      private IReferencedType _type;
      [Required]
      public IReferencedType Type
      {
         get { return _type; }
         set { SetProperty(ref _type, value); }
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
      private Operator _operator;
      public Operator Operator
      {
         get { return _operator; }
         set { SetProperty(ref _operator, value); }
      }
      private bool _isStatic;
      public bool IsStatic
      {
         get { return _isStatic; }
         set { SetProperty(ref _isStatic, value); }
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
