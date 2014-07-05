using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom.BasesAndBaseHelpers
{
    internal static class SameIntentHelpers<T, TSyntax, TSymbol>
            where TSyntax : SyntaxNode
            where TSymbol : ISymbol
            where T : class, IDom<T>
    {
        internal static bool CheckSameIntent(T one, T other, bool includePublicAnnotations)
        {
            var item = one as RDomBase<T, TSyntax, TSymbol>;
            if (one == null) { throw new InvalidOperationException(); }
            var rDomOther = other as RDomBase<T, TSyntax, TSymbol>;
            if (rDomOther == null) { return false;}
            if (one.Name != rDomOther.Name) { return false;}
            //if (!CheckSameIntentNamespaceNames(one, other)) { return false;}
            if (!CheckSameIntentAccessModifier(one, other)) { return false;}
            if (!CheckSameIntentAttributes(one, other)) { return false;}
            if (!CheckSameIntentAccessModifier(one, other)) { return false;}
            if (!CheckSameIntentStaticModfier(one, other)) { return false;}
            if (!CheckSameIntentReturnType(one, other)) { return false;}
            if (!CheckSameIntentPropertyOrMethod(one, other)) { return false;}

            return true;
        }
        private static bool CheckSameIntentNamespaceNames(T one, T other)
        {
            if (one.OuterName != other.OuterName) return false;
            var item = one as IHasNamespace;
            if (item != null)
            {
                var otherItem = other as IHasNamespace;
                if (item.Namespace != otherItem.Namespace) return false;
                if (item.QualifiedName != otherItem.QualifiedName) return false;
            }
            return true;
        }

        private static bool CheckSameIntentAttributes(T one, T other)
        {
            var item = one as IHasAttributes;
            if (item != null)
            {
                var otherItem = other as IHasAttributes;
                var attributes = item.Attributes;
                var otherAttributes = otherItem.Attributes;
                if (attributes != null || otherAttributes != null)
                {
                    if (attributes == null && otherAttributes != null) return false;
                    if (attributes != null && otherAttributes == null) return false;
                    if (attributes.Count() != otherAttributes.Count()) return false;
                    foreach (var attribute in attributes)
                    {
                        // TODO: Consider multiple attributes of the same name and values/attribute type
                        var otherAttribute = otherAttributes.Where(x => x.Name == attribute.Name).FirstOrDefault();
                        if (otherAttribute == null) return false;
                        if (!attribute.SameIntent(otherAttribute)) return false;
                    }
                }
            }
            return true;
        }

        private static bool CheckSameIntentAccessModifier(T one, T other)
        {
            var item = one as IHasAccessModifier;
            if (item != null)
            {
                var otherItem = other as IHasAccessModifier;
                if (item.AccessModifier != otherItem.AccessModifier) return false;
            }
            return true;
        }

        private static bool CheckSameIntentStaticModfier(T one, T other)
        {
            var item = one as ICanBeStatic;
            if (item != null)
            {
                var otherItem = other as ICanBeStatic;
                if (item.IsStatic != otherItem.IsStatic) return false;
            }
            return true;
        }

        private static bool CheckSameIntentReturnType(T one, T other)
        {
            var item = one as IHasReturnType;
            if (item != null)
            {
                var otherItem = other as IHasReturnType;
                if (!item.ReturnType.SameIntent(otherItem.ReturnType)) return false;
            }
            return true;
        }

        private static bool CheckSameIntentPropertyOrMethod(T one, T other)
        {
            var item = one as IPropertyOrMethod;
            if (item != null)
            {
                var otherItem = other as IPropertyOrMethod;
                if (item.IsAbstract != otherItem.IsAbstract) return false;
                if (item.IsOverride != otherItem.IsOverride) return false;
                if (item.IsSealed != otherItem.IsSealed) return false;
                if (item.IsVirtual != otherItem.IsVirtual) return false;
            }
            return true;
        }
    }
}
