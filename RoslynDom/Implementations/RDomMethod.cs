using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomMethod : RDomBase<IMethod, IMethodSymbol>, IMethod
   {
      private RDomCollection<IParameter> _parameters;
      private RDomCollection<ITypeParameter> _typeParameters;
      private RDomCollection<IStatementAndDetail> _statements;
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomMethod( string name, string returnTypeName, AccessModifier declaredAccessModifier = AccessModifier.Private,
               bool isAbstract = false, bool isVirtual = false, bool isOverride = false, bool isSealed = false,
               bool isNew = false, bool isStatic = false,
               bool isExtensionMethod = false)
          : this(name, declaredAccessModifier, isAbstract, isVirtual,
                     isOverride, isSealed, isNew, isStatic, isExtensionMethod)
      {
         _returnType = new RDomReferencedType(this, returnTypeName);
      }

      public RDomMethod(string name, IReferencedType returnType, AccessModifier declaredAccessModifier = AccessModifier.Private,
               bool isAbstract = false, bool isVirtual = false, bool isOverride = false, bool isSealed = false,
               bool isNew = false, bool isStatic = false,
               bool isExtensionMethod = false)
          : this(name, declaredAccessModifier, isAbstract, isVirtual,
                     isOverride, isSealed, isNew, isStatic, isExtensionMethod)
      {
         _returnType = returnType;
      }

      private RDomMethod( string name,  AccessModifier declaredAccessModifier = AccessModifier.Private,
               bool isAbstract = false, bool isVirtual = false, bool isOverride = false, bool isSealed = false,
               bool isNew = false, bool isStatic = false,
               bool isExtensionMethod = false)
            : base()
      {
         Initialize();
         _name = name;
         DeclaredAccessModifier = declaredAccessModifier; // Must use the setter here!
         _isAbstract = isAbstract;
         _isVirtual = isVirtual;
         _isOverride = isOverride;
         _isSealed = isSealed;
         _isNew = isNew;
         _isStatic = isStatic;
         _isExtensionMethod = isExtensionMethod;
      }

      public RDomMethod(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
     "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomMethod(RDomMethod oldRDom)
           : base(oldRDom)
      {
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         _parameters = oldRDom.Parameters.Copy(this);
         _typeParameters = oldRDom.TypeParameters.Copy(this);
         _statements = oldRDom.StatementsAll.Copy(this);

         _name = oldRDom.Name;
         _returnType = oldRDom.ReturnType.Copy();
         _accessModifier = oldRDom.AccessModifier;
         _declaredAccessModifier = oldRDom.DeclaredAccessModifier;
         _isAbstract = oldRDom.IsAbstract;
         _isVirtual = oldRDom.IsVirtual;
         _isOverride = oldRDom.IsOverride;
         _isSealed = oldRDom.IsSealed;
         _isStatic = oldRDom.IsStatic;
         _isNew = oldRDom.IsNew;
         _isExtensionMethod = oldRDom.IsExtensionMethod;
      }

      private void Initialize()
      {
         _typeParameters = new RDomCollection<ITypeParameter>(this);
         _parameters = new RDomCollection<IParameter>(this);
         _statements = new RDomCollection<IStatementAndDetail>(this);
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

      private IReferencedType _returnType;
      [Required]
      public IReferencedType ReturnType
      {
         get { return _returnType; }
         set { SetProperty(ref _returnType, value); }
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

      private bool _isAbstract;
      public bool IsAbstract
      {
         get { return _isAbstract; }
         set { SetProperty(ref _isAbstract, value); }
      }

      private bool _isVirtual;
      public bool IsVirtual
      {
         get { return _isVirtual; }
         set { SetProperty(ref _isVirtual, value); }
      }

      private bool _isOverride;
      public bool IsOverride
      {
         get { return _isOverride; }
         set { SetProperty(ref _isOverride, value); }
      }

      private bool _isSealed;
      public bool IsSealed
      {
         get { return _isSealed; }
         set { SetProperty(ref _isSealed, value); }
      }

      private bool _isNew;
      public bool IsNew
      {
         get { return _isNew; }
         set { SetProperty(ref _isNew, value); }
      }

      private bool _isStatic;
      public bool IsStatic
      {
         get { return _isStatic; }
         set { SetProperty(ref _isStatic, value); }
      }

      private bool _isExtensionMethod;
      public bool IsExtensionMethod
      {
         get { return _isExtensionMethod; }
         set { SetProperty(ref _isExtensionMethod, value); }
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

      public RDomCollection<ITypeParameter> TypeParameters
      { get { return _typeParameters; } }

      public RDomCollection<IParameter> Parameters
      { get { return _parameters; } }

      public RDomCollection<IStatementAndDetail> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      public bool HasBlock
      {
         get { return true; }
         set { }
      }

      public MemberKind MemberKind
      { get { return MemberKind.Method; } }
   }
}
