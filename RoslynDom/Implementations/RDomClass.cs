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
 public RDomClass (string _baseTypeName,bool  _isAbstract,bool  _isSealed,bool  _isStatic ) : this (new RDomReferencedType(_baseTypeName),_isAbstract,_isSealed,_isStatic ) { BaseType=_baseType; BaseType=_baseType; IsAbstract=_isAbstract; BaseType=_baseType; IsAbstract=_isAbstract; IsSealed=_isSealed; BaseType=_baseType; IsAbstract=_isAbstract; IsSealed=_isSealed; IsStatic=_isStatic; } public RDomClass (IReferencedType  _baseType,bool  _isAbstract,bool  _isSealed,bool  _isStatic ) : this (null,null,null ) { BaseType=_baseType; BaseType=_baseType; IsAbstract=_isAbstract; BaseType=_baseType; IsAbstract=_isAbstract; IsSealed=_isSealed; BaseType=_baseType; IsAbstract=_isAbstract; IsSealed=_isSealed; IsStatic=_isStatic; }      public RDomClass(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model, MemberKind.Class, StemMemberKind.Class)
      { }

      internal RDomClass(RDomClass oldRDom)
         : base(oldRDom)
      {
         if (oldRDom.BaseType != null) { BaseType = oldRDom.BaseType.Copy(); }
         IsAbstract = oldRDom.IsAbstract;
         IsSealed = oldRDom.IsSealed;
         IsStatic = oldRDom.IsStatic;
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

      public IEnumerable<IConstructor> Constructors
      { get { return Members.OfType<IConstructor>(); } }
   }
}
