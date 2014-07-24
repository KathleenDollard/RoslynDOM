using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntent_Factory
    {
        // This is a hacked solution until I decide on DI approach
        private static IDictionary<Type, Type> lookup = new Dictionary<Type, Type>()
            {
                { typeof(IAttribute), typeof(SameIntent_IAttribute) },
                { typeof(IAttributeValue), typeof(SameIntent_IAttributeValue) },
                { typeof(IClass), typeof(SameIntent_IClass) },
                { typeof(IEnum), typeof(SameIntent_IEnum) },
                { typeof(IField), typeof(SameIntent_IField) },
                { typeof(IInterface), typeof(SameIntent_IInterface) },
                { typeof(IInvalidTypeMember), typeof(SameIntent_IInvalidTypeMember) },
                { typeof(IMethod), typeof(SameIntent_IMethod) },
                { typeof(INamespace), typeof(SameIntent_INamespace) },
                { typeof(IParameter), typeof(SameIntent_IParameter) },
                { typeof(IProperty), typeof(SameIntent_IProperty) },
                { typeof(IReferencedType), typeof(SameIntent_IReferencedType) },
                { typeof(IRoot), typeof(SameIntent_IRoot) },
                { typeof(IStructure), typeof(SameIntent_IStructure) },
                { typeof(ITypeParameter), typeof(SameIntent_ITypeParameter) } ,
                { typeof(IUsingDirective), typeof(SameIntent_IUsingDirective) }
             };

        public static  ISameIntent<T> SameIntent<T>()
        {
            Type type;
            if (lookup.TryGetValue(typeof(T), out type))
            {
                var newItem = Activator.CreateInstance(type) as ISameIntent<T>;
                return newItem;
            }
            return null;
        }
    }
}
