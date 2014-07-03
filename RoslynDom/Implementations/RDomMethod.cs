using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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
            var newParameters = RDomBase<IParameter>
                                 .CopyMembers(oldRDom._parameters.Cast<RDomParameter>());
            _parameters = newParameters;
            var newTypeParameters = RDomBase<ITypeParameter>
                                 .CopyMembers(oldRDom._typeParameters.Cast<RDomTypeParameter>());
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
            AccessModifier = (AccessModifier)Symbol.DeclaredAccessibility;
            ReturnType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.ReturnType);
            IsAbstract = Symbol.IsAbstract;
            IsVirtual = Symbol.IsVirtual;
            IsOverride = Symbol.IsOverride;
            IsSealed = Symbol.IsSealed;
            IsStatic = Symbol.IsStatic;
            IsExtensionMethod = TypedSymbol.IsExtensionMethod;
        }

        public override bool SameIntent(IMethod other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (ReturnType.QualifiedName  != other.ReturnType.QualifiedName ) return false;
            if (IsAbstract != other.IsAbstract) return false;
            if (IsSealed != other.IsSealed) return false;
            if (IsStatic != other.IsStatic) return false;
            if (IsVirtual != other.IsVirtual) return false;
            if (IsExtensionMethod != other.IsExtensionMethod) return false;
            if (!CheckSameIntentChildList(TypeParameters, other.TypeParameters)) return false;
            if (!CheckSameIntentChildList(Parameters, other.Parameters)) return false;
            return true;
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
