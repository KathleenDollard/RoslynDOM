using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentCommon
    {
        public  bool CheckSameIntent<T>(T one, T other, bool includePublicAnnotations)
            where T :IDom
        {
            if (one == null) { throw new InvalidOperationException(); }
            if (one.Name != other.Name) { return false; }
            //if (!CheckSameIntentNamespaceNames(one, other)) { return false;}
            if (!CheckSameIntentAccessModifier(one, other)) { return false; }
            if (!CheckSameIntentAttributes(one, other)) { return false; }
            if (!CheckSameIntentAccessModifier(one, other)) { return false; }
            if (!CheckSameIntentStaticModfier(one, other)) { return false; }
            if (!CheckSameIntentReturnType(one, other)) { return false; }
            if (!CheckSameIntentPropertyOrMethod(one, other)) { return false; }

            return true;
        }

        public   bool CheckSameIntentNamespaceNames<T>(T one, T other)
            where T : IDom
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

        private static bool CheckSameIntentAttributes<T>(T one, T other)
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

        private static bool CheckSameIntentAccessModifier<T>(T one, T other)
        {
            var item = one as IHasAccessModifier;
            if (item != null)
            {
                var otherItem = other as IHasAccessModifier;
                if (item.AccessModifier != otherItem.AccessModifier) return false;
            }
            return true;
        }

        private static bool CheckSameIntentStaticModfier<T>(T one, T other)
        {
            var item = one as ICanBeStatic;
            if (item != null)
            {
                var otherItem = other as ICanBeStatic;
                if (item.IsStatic != otherItem.IsStatic) return false;
            }
            return true;
        }

        private static bool CheckSameIntentReturnType<T>(T one, T other)
        {
            var item = one as IHasReturnType;
            if (item != null)
            {
                var otherItem = other as IHasReturnType;
                if (!item.ReturnType.SameIntent(otherItem.ReturnType)) return false;
            }
            return true;
        }

        private static bool CheckSameIntentPropertyOrMethod<T>(T one, T other)
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
