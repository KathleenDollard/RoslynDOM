using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntent_Factory
    {
        // TODO: Update this to use DI
        private static IDictionary<Type, Type> lookup = new Dictionary<Type, Type>()
            {
                { typeof(IAttribute), typeof(SameIntent_IAttribute) },
                { typeof(IAttributeValue), typeof(SameIntent_IAttributeValue) },
                { typeof(IClass), typeof(SameIntent_IClass) },
                { typeof(IEnum), typeof(SameIntent_IEnum) },
                { typeof(IEnumValue), typeof(SameIntent_IEnumValue) },
                { typeof(IField), typeof(SameIntent_IField) },
                { typeof(IInterface), typeof(SameIntent_IInterface) },
                { typeof(IInvalidMember), typeof(SameIntent_IInvalidTypeMember) },
                { typeof(IMethod), typeof(SameIntent_IMethod) },
                { typeof(INamespace), typeof(SameIntent_INamespace) },
                { typeof(IParameter), typeof(SameIntent_IParameter) },
                { typeof(IProperty), typeof(SameIntent_IProperty) },
                { typeof(IReferencedType), typeof(SameIntent_IReferencedType) },
                { typeof(IRoot), typeof(SameIntent_IRoot) },
                { typeof(IStructure), typeof(SameIntent_IStructure) },
                { typeof(ITypeParameter), typeof(SameIntent_ITypeParameter) } ,
                { typeof(IUsingDirective), typeof(SameIntent_IUsingDirective) },
                { typeof(IStatement), typeof(SameIntent_IStatement) },
                { typeof(IExpression), typeof(SameIntent_IExpression) }
             };

        public static  ISameIntent<T> SameIntent<T>()
        {
            Type type = null;
            if (!lookup.TryGetValue(typeof(T), out type))
            {
                if (typeof(IStatement) .IsAssignableFrom(typeof(T)))
                { type = typeof(SameIntent_IStatement); }
            }
            if (type != null)
            { 
                var newItem = Activator.CreateInstance(type) as ISameIntent<T>;
                return newItem;
            }
            return null;
        }
    }
}
