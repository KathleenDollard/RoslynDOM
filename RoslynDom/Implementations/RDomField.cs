using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using cm=System.ComponentModel;
 using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{

   /// <summary></summary>
   /// <returns></returns>
   /// <remarks>
   /// Field assignments in the form "Type x, y, z" are not yet supported
   /// and when they are they will be loaded as separate fields (rather
   /// obviously). At that point, the variable declaration will need to be held in
   /// the class.
   /// </remarks>
   public class RDomField : RDomBase<IField, IFieldSymbol>, IField
   {
      private AttributeCollection _attributes = new AttributeCollection();

      /// <summary>
      /// Constructor to use when creating a RoslynDom from scratch
      /// </summary>
      /// <param name="name">
      /// Name of the field
      /// </param>
      /// <param name="typeName">
      /// Type name of the field
      /// </param>
      /// <param name="accessModifier">
      /// The accessibilty (scope) modifier. Default is the most restrictive scope, private.
      /// </param>
      /// <param name="isStatic">
      /// Pass true for an static class
      /// </param>
      /// <param name="isNew">
      /// Pass true for an new class
      /// </param>
      public RDomField(string name, string typeName, AccessModifier accessModifier = AccessModifier.Private,
                      bool isStatic = false, bool isNew = false, bool isReadOnly = false,
                      bool isVolatile = false, bool isConstant = false)
       : this(name, new RDomReferencedType(typeName), accessModifier, isStatic, isNew, isReadOnly, isVolatile, isConstant)
      { }

      /// <summary>
      /// Constructor to use when creating a RoslynDom from scratch
      /// </summary>
      /// <param name="name">
      /// Name of the field
      /// </param>
      /// <param name="typeName">
      /// Type name of the field</param>
      /// <param name="declaredAccessModifier">
      /// The accessibilty (scope) modifier. Default is the most restrictive scope, private.
      /// </param>
      /// <param name="isStatic">
      /// Pass true for an static class
      /// </param>
      /// <param name="isNew">
      /// Pass true for an new class
      /// </param>
      public RDomField(string name, IReferencedType type, AccessModifier declaredAccessModifier = AccessModifier.Private,
                      bool isStatic = false, bool isNew = false, bool isReadOnly = false,
                      bool isVolatile = false, bool isConstant = false)
       : this(null, null, null)
      {
         Name = name;
         ReturnType = type;
         DeclaredAccessModifier = declaredAccessModifier;
         IsStatic = isStatic;
         IsNew = isNew;
         IsReadOnly = isReadOnly;
         IsVolatile = isVolatile;
         IsConstant = isConstant;
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.EndOfLine, "", "\r\n", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Identifier, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Static, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.NewSlot, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Readonly, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Constant, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Volatile, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.EqualsAssignmentOperator, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Public, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Private, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Protected, "", " ", ""));
         this.Whitespace2Set.Add(new Whitespace2(LanguageElement.Internal, "", " ", ""));
      }

      [cm.EditorBrowsable(cm.EditorBrowsableState.Never)]
      public RDomField(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomField(RDomField oldRDom)
          : base(oldRDom)
      {
         Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
         AccessModifier = oldRDom.AccessModifier;
         DeclaredAccessModifier = oldRDom.DeclaredAccessModifier;
         ReturnType = oldRDom.ReturnType;
         IsStatic = oldRDom.IsStatic;
         IsReadOnly = oldRDom.IsReadOnly;
         IsConstant = oldRDom.IsConstant;
         IsVolatile = oldRDom.IsVolatile;
         IsNew = oldRDom.IsNew;
         Initializer = oldRDom.Initializer == null
                         ? null
                         : oldRDom.Initializer.Copy();
      }

      public AttributeCollection Attributes
      { get { return _attributes; } }

      private string _name ;
      [Required]
      public string Name { get {return _name; }
set {SetProperty(ref _name, value); }}
      private IReferencedType _returnType ;
      [Required]
      public IReferencedType ReturnType { get {return _returnType; }
set {SetProperty(ref _returnType, value); }}
      private AccessModifier _accessModifier ;
      public AccessModifier AccessModifier { get {return _accessModifier; }
set {SetProperty(ref _accessModifier, value); }}
      private AccessModifier _declaredAccessModifier ;
      public AccessModifier DeclaredAccessModifier { get {return _declaredAccessModifier; }
set {SetProperty(ref _declaredAccessModifier, value); }}
      private bool _isStatic ;
      public bool IsStatic { get {return _isStatic; }
set {SetProperty(ref _isStatic, value); }}
      private bool _isNew ;
      public bool IsNew { get {return _isNew; }
set {SetProperty(ref _isNew, value); }}
      private bool _isReadOnly ;
      public bool IsReadOnly { get {return _isReadOnly; }
set {SetProperty(ref _isReadOnly, value); }}
      private bool _isVolatile ;
      public bool IsVolatile { get {return _isVolatile; }
set {SetProperty(ref _isVolatile, value); }}
      private bool _isConstant ;
      public bool IsConstant { get {return _isConstant; }
set {SetProperty(ref _isConstant, value); }}
      private IExpression _initializer ;
      public IExpression Initializer { get {return _initializer; }
set {SetProperty(ref _initializer, value); }}
      private IStructuredDocumentation _structuredDocumentation ;
      public IStructuredDocumentation StructuredDocumentation { get {return _structuredDocumentation; }
set {SetProperty(ref _structuredDocumentation, value); }}
      private string _description ;
      public string Description { get {return _description; }
set {SetProperty(ref _description, value); }}

      public MemberKind MemberKind
      {
         get { return MemberKind.Field; }
      }
   }
}
