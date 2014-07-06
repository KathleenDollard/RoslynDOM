using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomMethod : RDomBase<IMethod, MethodDeclarationSyntax, IMethodSymbol>, IMethod
    {
        private IEnumerable<IParameter> _parameters;
        private IEnumerable<ITypeParameter> _typeParameters;

        internal RDomMethod(
            MethodDeclarationSyntax rawItem,
            IEnumerable<IParameter> parameters,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            _parameters = parameters;
            Initialize();
        }

        internal RDomMethod(RDomMethod oldRDom)
             : base(oldRDom)
        {
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            _parameters = newParameters;
            var newTypeParameters = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
            _typeParameters = newTypeParameters;

            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsAbstract = oldRDom.IsAbstract;
            IsVirtual = oldRDom.IsVirtual;
            IsOverride = oldRDom.IsOverride;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
            IsExtensionMethod = oldRDom.IsExtensionMethod;

        }

        protected override void Initialize()
        {
            base.Initialize();
            _typeParameters = this.TypedSymbol.TypeParametersFrom();
            AccessModifier = GetAccessibility();
            ReturnType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.ReturnType);
            IsAbstract = Symbol.IsAbstract;
            IsVirtual = Symbol.IsVirtual;
            IsOverride = Symbol.IsOverride;
            IsSealed = Symbol.IsSealed;
            IsStatic = Symbol.IsStatic;
            IsExtensionMethod = TypedSymbol.IsExtensionMethod;
        }

          public override MethodDeclarationSyntax BuildSyntax()
        {
            var nameSyntax = SyntaxFactory.Identifier(Name);
            var returnType = ((RDomReferencedType)ReturnType).BuildSyntax();
            var modifiers = BuildModfierSyntax();
            //var typeParameters = BuildTypeParameterList();
            //var constraintClauses = BuildConstraintClauses();
            //var members = BuildMembers();

            var node = SyntaxFactory.MethodDeclaration(returnType, nameSyntax)
                            .WithModifiers(modifiers);
            var attributesLists = BuildAttributeListSyntax();
            if (attributesLists.Any()) { node = node.WithAttributeLists(attributesLists); }
            return (MethodDeclarationSyntax)RoslynUtilities.Format(node);
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }
        public IReferencedType ReturnType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsSealed { get; set; }
        public bool IsStatic { get; set; }

        public bool IsExtensionMethod { get; set; }

        public IEnumerable<ITypeParameter> TypeParameters
        { get { return _typeParameters; } }

        public IEnumerable<IParameter> Parameters
        { get { return _parameters; } }

        public MemberType MemberType
        { get { return MemberType.Method; } }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            {
                return ReturnType.QualifiedName;
            }
            return base.RequestValue(name);
        }
    }
}
