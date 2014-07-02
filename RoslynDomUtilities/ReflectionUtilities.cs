using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public static class ReflectionUtilities
    {
        public static bool CanGetProperty(object instance, string propertyName)
        {
            if (instance == null) return false;
            var type = instance.GetType().GetTypeInfo();
            var propInfo = type.GetProperty(propertyName);
            if (propInfo == null) return false;
            return (propInfo.CanRead);
        }

        public static bool CanSetProperty(object instance, string propertyName)
        {
            if (instance == null) return false;
            var type = instance.GetType().GetTypeInfo();
            var propInfo = type.GetProperty(propertyName);
            if (propInfo == null) return false;
            return (propInfo.CanWrite);
        }

        public static object GetPropertyValue(object instance, string propertyName)
        {
            if (instance == null) return false;
            var type = instance.GetType().GetTypeInfo();
            var propInfo = type.GetProperty(propertyName);
            return propInfo.GetValue(instance);
        }

    }
}
