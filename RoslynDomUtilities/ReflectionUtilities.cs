using System;
using System.Linq;
using System.Reflection;

namespace RoslynDom.Common
{
    public static class ReflectionUtilities
    {
        public static bool CanGetProperty(object instance, string propertyName)
        {
            if (instance == null) throw new InvalidOperationException();
            var type = instance.GetType().GetTypeInfo();
            var propInfo = type.GetProperty(propertyName);
            if (propInfo == null) return false;
            return (propInfo.CanRead);
        }

        public static bool CanSetProperty(object instance, string propertyName)
        {
            if (instance == null) throw new InvalidOperationException();
            var type = instance.GetType().GetTypeInfo();
            var propInfo = type.GetProperty(propertyName);
            if (propInfo == null) return false;
            return (propInfo.CanWrite);
        }

        public static object GetPropertyValue(object instance, string propertyName)
        {
            if (instance == null) throw new InvalidOperationException();
            var type = instance.GetType().GetTypeInfo();
            var propInfo = type.GetProperty(propertyName);
            return propInfo.GetValue(instance);
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
            var methodInfo = type.GetMethod(methodName, parameterTypes);
            return methodInfo;
        }
    }
}
