using System;
using System.Linq;
using System.Reflection;

namespace RoslynDom.Common
{
    public static class ReflectionUtilities
    {
        public static bool CanGetProperty(object instance, string propertyName)
        {
            var propInfo = GetPropertyInfo(instance, propertyName);
            if (propInfo == null) return false;
            return (propInfo.CanRead);
        }

        public static bool CanSetProperty(object instance, string propertyName)
        {
            var propInfo = GetPropertyInfo(instance, propertyName);
            if (propInfo == null) return false;
            return (propInfo.CanWrite);
        }

        public static object GetPropertyValue(object instance, string propertyName)
        {
            var propInfo = GetPropertyInfo(instance, propertyName);
            return propInfo.GetValue(instance);
        }

        public static void SetPropertyValue(object instance, string propertyName, object value)
        {
            var propInfo = GetPropertyInfo(instance, propertyName);
            propInfo.SetValue(instance, value);
        }

        private static PropertyInfo GetPropertyInfo(object instance, string propertyName)
        {
            if (instance == null) throw new InvalidOperationException();
            var type = instance.GetType().GetTypeInfo();
            var propInfo = type.GetProperty(propertyName);
            return propInfo;
        }

        public static MethodInfo MakeGenericMethod(Type type, string methodName, params Type[] genericTypes)
        {
            var method = type
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x => x.Name == methodName)
                .First();
            return method.MakeGenericMethod(genericTypes);
        }

        public static MethodInfo FindMethod(Type type, string methodName, params Type[] parameterTypes)
        {
            return FindMethod(type, methodName, false, parameterTypes);
        }

        public static MethodInfo FindMethod(Type type, string methodName, bool mightBePrivate, params Type[] parameterTypes)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
            if (mightBePrivate) { bindingFlags = bindingFlags | BindingFlags.NonPublic; }
            var methodInfo = type.GetMethod(methodName, bindingFlags, null, parameterTypes, null);
            return methodInfo;
        }

        public static Type MakeGenericType(Type openType, params Type[] typeArguments)
        {
            var newType = openType.MakeGenericType(typeArguments);
            return newType;
        }



    }
}
