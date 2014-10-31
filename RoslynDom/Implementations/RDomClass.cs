using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   /// <summary></summary>
   /// <remarks></remarks>
   public class RDomClass : RDomBaseType<IClass>, IClass
   {
      public RDomClass(string name, string baseTypeName = null,AccessModifier accessModifier = AccessModifier.Private, 
                   bool isAbstract = false, bool isSealed = false, bool isStatic = false, bool isPartial = false)
          : this(name,new RDomReferencedType(baseTypeName), accessModifier,  isAbstract, isSealed, isStatic, isPartial)
      { }

      public RDomClass(string name, IReferencedType baseType, AccessModifier accessModifier = AccessModifier.Private,
                   bool isAbstract = false, bool isSealed = false, bool isStatic = false, bool isPartial = false)
      : base(name, accessModifier, MemberKind.Class, StemMemberKind.Class)
      {
         _baseType = baseType;
         _isAbstract = isAbstract;
         _isSealed = isSealed;
         _isStatic = isStatic;
         _isPartial = isPartial;
      }

      public RDomClass(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model, MemberKind.Class, StemMemberKind.Class)
      { }

      internal RDomClass(RDomClass oldRDom)
         : base(oldRDom)
      {
         if (oldRDom.BaseType != null) { _baseType = oldRDom.BaseType.Copy(); }
         _isAbstract = oldRDom.IsAbstract;
         _isSealed = oldRDom.IsSealed;
         _isStatic = oldRDom.IsStatic;
         _isPartial = oldRDom.IsPartial;
      }

      public IEnumerable<IField> Fields
      { get { return Members.OfType<IField>().ToList(); } }

      public IEnumerable<IOperator> Operators
      { get { return Members.OfType<IOperator>().ToList(); } }

      public IEnumerable<IConversionOperator> ConversionOperators
      { get { return Members.OfType<IConversionOperator>().ToList(); } }

      public IDestructor Destructor
      { get { return Members.OfType<IDestructor>().SingleOrDefault(); } }

      public IEnumerable<IClass> Classes
      { get { return Members.OfType<IClass>().ToList(); } }

      public IEnumerable<IType> Types
      { get { return Members.OfType<IType>().ToList(); } }

      public IEnumerable<IStructure> Structures
      { get { return Members.OfType<IStructure>().ToList(); } }

      public IEnumerable<IInterface> Interfaces
      { get { return Members.OfType<IInterface>().ToList(); } }

      public IEnumerable<IEnum> Enums
      { get { return Members.OfType<IEnum>().ToList(); } }

      private IReferencedType _baseType;
      public IReferencedType BaseType
      {
         get { return _baseType; }
         set { SetProperty(ref _baseType, value); }
      }

      private bool _isAbstract;
      public bool IsAbstract
      {
         get { return _isAbstract; }
         set { SetProperty(ref _isAbstract, value); }
      }

      private bool _isSealed;
      public bool IsSealed
      {
         get { return _isSealed; }
         set { SetProperty(ref _isSealed, value); }
      }

      private bool _isStatic;
      public bool IsStatic
      {
         get { return _isStatic; }
         set { SetProperty(ref _isStatic, value); }
      }

      private bool _isPartial;
      public bool IsPartial
      {
         get { return _isPartial; }
         set { SetProperty(ref _isPartial, value); }
      }

      public IEnumerable<IConstructor> Constructors
      { get { return Members.OfType<IConstructor>(); } }
   }
}
