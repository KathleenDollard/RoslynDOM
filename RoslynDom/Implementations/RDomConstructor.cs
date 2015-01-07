using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RoslynDom
{
   public class RDomConstructor : RDomBase<IConstructor, IMethodSymbol>, IConstructor
   {
      private RDomCollection<IParameter> _parameters;
      private RDomCollection<IArgument> _initializationArguments;
      private RDomCollection<IStatementAndDetail> _statements;
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomConstructor(string name, AccessModifier declaredAccessModifier = AccessModifier.Private,
                      bool isStatic = false, ConstructorInitializerType constructorInitializerType = ConstructorInitializerType.None)
         : base()
      {
         Initialize();
         _name = name;
         DeclaredAccessModifier = declaredAccessModifier;
         _isStatic = isStatic;
         _constructorInitializerType = constructorInitializerType;
      }

      public RDomConstructor(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomConstructor(RDomConstructor oldRDom)
         : base(oldRDom)
      {
         Initialize();
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         _parameters = oldRDom.Parameters.Copy(this);
         _statements = oldRDom.StatementsAll.Copy(this);
         _initializationArguments = oldRDom.InitializationArguments.Copy(this);
         _constructorInitializerType = oldRDom.ConstructorInitializerType;
         _name = oldRDom.Name;
         _accessModifier = oldRDom.AccessModifier;
         _declaredAccessModifier = oldRDom.DeclaredAccessModifier;
         _isStatic = oldRDom.IsStatic;
      }

      public bool AddOrMoveMember(IDom item)
      { return _statements.AddOrMove(item); }

      public bool RemoveMember(IDom item)
      { return _statements.Remove(item); }

      public bool InsertOrMoveMember(int index, IDom item)
      { return _statements.InsertOrMove(index, item); }

      public IEnumerable<IDom> GetMembers()
      { return _statements.ToList(); }

      private void Initialize()
      {
         _parameters = new RDomCollection<IParameter>(this);
         _statements = new RDomCollection<IStatementAndDetail>(this);
         _initializationArguments = new RDomCollection<IArgument>(this);
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
         set
         {
            SetProperty(ref _declaredAccessModifier, value);
            AccessModifier = value;
         }
      }

      private bool _isStatic;

      public bool IsStatic
      {
         get { return _isStatic; }
         set { SetProperty(ref _isStatic, value); }
      }

      private ConstructorInitializerType _constructorInitializerType;

      public ConstructorInitializerType ConstructorInitializerType
      {
         get { return _constructorInitializerType; }
         set { SetProperty(ref _constructorInitializerType, value); }
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

      public RDomCollection<IArgument> InitializationArguments
      { get { return _initializationArguments; } }

      public RDomCollection<IParameter> Parameters
      { get { return _parameters; } }

      public RDomCollection<IStatementAndDetail> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      private bool _hasBlock;

      public bool HasBlock
      {
         get { return true; return _hasBlock; }
         set { SetProperty(ref _hasBlock, value); }
      }

      public MemberKind MemberKind
      { get { return MemberKind.Constructor; } }
   }
}