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
      private RDomCollection<IStatementCommentWhite> _statements;
      private AttributeCollection _attributes = new AttributeCollection();

      public RDomMethod(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
     "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomMethod(RDomMethod oldRDom)
           : base(oldRDom)
      {
         Initialize();
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
         Parameters.AddOrMoveRange(newParameters);
         var newTypeParameters = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
         TypeParameters.AddOrMoveRange(newTypeParameters);
         var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
         StatementsAll.AddOrMoveRange(newStatements);

         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         ReturnType = oldRDom.ReturnType;
         IsAbstract = oldRDom.IsAbstract;
         IsVirtual = oldRDom.IsVirtual;
         IsOverride = oldRDom.IsOverride;
         IsSealed = oldRDom.IsSealed;
         IsStatic = oldRDom.IsStatic;
         IsNew = oldRDom.IsNew;
         IsExtensionMethod = oldRDom.IsExtensionMethod;
      }

      private void Initialize()
      {
         _typeParameters = new RDomCollection<ITypeParameter>(this);
         _parameters = new RDomCollection<IParameter>(this);
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
         set { SetProperty(ref _declaredAccessModifier, value); }
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

      public RDomCollection<IStatementCommentWhite> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      private bool hasBlock ;
      public bool HasBlock
      {
         get { return true; return hasBlock;}
         set { SetProperty(ref hasBlock, value);}
      }

      public MemberKind MemberKind
      { get { return MemberKind.Method; } }
   }
}
