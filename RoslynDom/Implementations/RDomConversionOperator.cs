using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RoslynDom
{
   public class RDomConversionOperator : RDomBase<IConversionOperator, IMethodSymbol>, IConversionOperator
   {
      private RDomCollection<IParameter> _parameters;
      private RDomCollection<IStatementAndDetail> _statements;
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomConversionOperator(string name, string typeName, bool isImplicit)
         : this(name, isImplicit)
      {
         _type = new RDomReferencedType(this, typeName);
      }

      public RDomConversionOperator(string name, IReferencedType type, bool isImplicit)
         : this(name, isImplicit)
      {
         _type = type;
      }

      private RDomConversionOperator(string name, bool isImplicit)
         : base()
      {
         Initialize();
         _name = name;
         _isImplicit = isImplicit;
      }

      public RDomConversionOperator(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
     "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomConversionOperator(RDomConversionOperator oldRDom)
         : base(oldRDom)
      {
         Initialize();
         _name = oldRDom.Name;
         _accessModifier = oldRDom.AccessModifier;
         _declaredAccessModifier = oldRDom.DeclaredAccessModifier;
         _isImplicit = oldRDom.IsImplicit;
         _isStatic = oldRDom.IsStatic;
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         _parameters = oldRDom.Parameters.Copy(this);
         _statements = oldRDom.StatementsAll.Copy(this);

         _type = oldRDom.Type.Copy();
      }

      private void Initialize()
      {
         _statements = new RDomCollection<IStatementAndDetail>(this);
         _parameters = new RDomCollection<IParameter>(this);
      }

      public bool AddOrMoveMember(IDom item)
      { return _statements.AddOrMove(item); }

      public bool RemoveMember(IDom item)
      { return _statements.Remove(item); }

      public bool InsertOrMoveMember(int index, IDom item)
      { return _statements.InsertOrMove(index, item); }

      public IEnumerable<IDom> GetMembers()
      { return _statements.ToList(); }

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
         set
         {
            SetProperty(ref _declaredAccessModifier, value);
            AccessModifier = value;
         }
      }

      private IReferencedType _type;

      public IReferencedType Type
      {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }

      private bool _isImplicit;

      public bool IsImplicit
      {
         get { return _isImplicit; }
         set { SetProperty(ref _isImplicit, value); }
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

      public RDomCollection<IStatementAndDetail> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      private bool _hasBlock;

      public bool HasBlock
      {
         get { return true; }
         set { SetProperty(ref _hasBlock, value); }
      }

      public MemberKind MemberKind
      { get { return MemberKind.ConversionOperator; } }
   }
}