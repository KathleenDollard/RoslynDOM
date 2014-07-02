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
        internal RDomMethod(
            MethodDeclarationSyntax rawItem,
            IEnumerable<IParameter> parametrs,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            _parameters = parametrs;
        }

        internal RDomMethod(RDomMethod oldRDom)
             : base(oldRDom)
        { }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public override bool SameIntent(IMethod other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (IsExtensionMethod != other.IsExtensionMethod) return false;
            if (!CheckSameIntentChildList(TypeParameters, other.TypeParameters)) return false;
            if (!CheckSameIntentChildList(Parameters, other.Parameters)) return false;
            return true;
        }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }

        public IReferencedType ReturnType
        {
            get
            {
                return new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.ReturnType);
            }
        }

        public bool IsAbstract
        { get { return Symbol.IsAbstract; } }

        public bool IsVirtual
        { get { return Symbol.IsVirtual; } }

        public bool IsOverride
        { get { return Symbol.IsOverride; } }

        public bool IsSealed
        { get { return Symbol.IsSealed; } }

        public bool IsStatic
        { get { return Symbol.IsStatic; } }

        public IEnumerable<ITypeParameter> TypeParameters
        { get { return this.TypedSymbol.TypeParametersFrom(); } }

        public IEnumerable<IParameter> Parameters
        { get { return _parameters; } }

        public bool IsExtensionMethod
        { get { return TypedSymbol.IsExtensionMethod; } }

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
