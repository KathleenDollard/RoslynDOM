using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomPropertyOrMethod<T, TSyntax, TSymbol> : RDomSyntaxNodeBase<T, TSyntax, TSymbol>
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
        where T : IDom<T>
    {
        internal RDomPropertyOrMethod(
               TSyntax rawItem,
               params PublicAnnotation[] publicAnnotations)
              : base(rawItem, publicAnnotations)
        { }

        internal RDomPropertyOrMethod(T oldRDom)
             : base(oldRDom)
        { }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public override bool SameIntent(T other, bool includePublicAnnotations)
        {
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (IsAbstract != other.IsAbstract
                || IsVirtual != other.IsVirtual
                || IsOverride != other.IsOverride
                || IsSealed != other.IsSealed
                || IsExtensionMethod != other.IsExtensionMethod
                || IsStatic != other.IsStatic)
                return false;
            // Watch the following line for errors. It might be a weak approach
            if (ReturnType.QualifiedName != other.ReturnType.QualifiedName) return false;
            if (!CheckSameIntentChildList(TypeParameters, other.TypeParameters)) return false;
            if (!CheckSameIntentChildList(Parameters, other.Parameters)) return false;
            return true;
        }

        public AccessModifier AccessModifier
        { get { return (AccessModifier)Symbol.DeclaredAccessibility; } }

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

    }
}
