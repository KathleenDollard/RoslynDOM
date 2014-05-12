using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;

namespace KadGen.Common
{
    public static class ReflectionSupport
    {
        public static object GetResultFromInstanceMethod(
            string methodName, object instance, params object[] parameters)
        {
            if (instance == null) { return null; }
            var method = GetMethod(instance.GetType(), methodName);
            return GetResultFromMethod(method, instance, parameters);
        }

        public static object GetResultFromStaticMethod(
            Type type, string methodName, params object[] parameters)
        {
            var method = GetMethod(type, methodName);
            return GetResultFromMethod(method, null, parameters);
        }

        public static object GetResultFromStaticMethod(
            MethodInfo method, params object[] parameters)
        {
            return GetResultFromMethod(method, null, parameters);
        }

        public static object GetResultFromMethod(
            MethodInfo method, object instance, params object[] parameters)
        {
            if (method == null) throw new InvalidOperationException("Could not find method");
            object val;
            try
            {
                val = method.Invoke(instance, parameters);
            }
            catch (TargetInvocationException ex)
            { throw UnwrapException(ex); }
            return val;
        }

        private static Exception UnwrapException(Exception ex)
        {
            var ret = ex;
            while (ret.GetType() == typeof(TargetInvocationException)
                && ret.InnerException != null)
            { ret = ret.InnerException; }
            return ret;
        }

        public static MethodInfo GetMethod(Type type, string methodName, params Type[] types)
        {
            var methods = type
                  .GetTypeInfo()
                  .DeclaredMethods
                  .Where(x => x.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)
                     && MethodParametersOfType(x.GetParameters(), types));
            if (methods.Count() != 1) { return null; }
             return methods.First(); 
        }

        private static bool MethodParametersOfType(IEnumerable<ParameterInfo> parameters,
           Type[] types)
        {
            if (types.Length != parameters.Count()) { return false; }
            var paramTypes = parameters.Select(x => x.ParameterType).ToArray();
            // This currently makes no attempt to resolve overloads
            for (int i = 0; i < types.Length; i++)
            {
                if (paramTypes[i] != types[i]) { return false; }
            }
            return true;
        }

        public static MethodInfo MakeMethod(Type type, string methodName, params Type[] genericTypes)
        {
            var method = type
                .GetTypeInfo()
                .DeclaredMethods
                .Where(x => x.Name == methodName)
                .First();
            return method.MakeGenericMethod(genericTypes);
        }

        public static bool HasAttribute(this Type type, Type attributeType)
        {
            if (type == null) { return false; }
            if (attributeType == null) { return false; }
            return type.GetCustomAttributes(attributeType, true).Length > 0;
        }

        public static object GetAttributeForEnumValue(Type type, Type attributeType, object value)
        {
            if (type == null) { return null; }
            if (attributeType == null) { return null; }
            // FUTURE: Not localizing because current usage is to create C# code. Could be localized via convention of a named parameter on such as Culture and CultureValue, assuming multiplicty was not otherwise required
            if (!type.IsEnum) throw new InvalidOperationException("Must run on enum types");
            var fields = type.GetTypeInfo().DeclaredFields.Where(x => x.Name == value.ToString());
            if (fields.Count() == 0) return null;
            var field = fields.First();
            var attributes = field.CustomAttributes.Where(x => x.AttributeType == attributeType);
            if (attributes.Count() == 0) return null;
            return attributes.First().ConstructorArguments.First().Value;
        }

        public static string CultureSpecificToString(
            Type type, object value, IFormatProvider formatProvider)
        {
            if (type == null) { return null; }
            System.Reflection.MethodInfo method = type.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
            if ((method == null))

            {
                // Aggregate exception can be used to test this. I am currently 
                // leaving this an exception so scenarios are evaluated. I trust
                // that types in the framework have been considered and only lack 
                // the overload when appropriate. I do not trust external code. 
                // This may not be the best decision, so this test helps the question
                // be revisited.
                throw new InvalidOperationException();
                //return objectToConvert.ToString();
            }
            else
            {
                return ((string)(method.Invoke(value, new object[] {
                                formatProvider })));
            }
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(TypeInfo typeInfo, Type stopClass = null)
        {
            if (typeInfo == null) { return null; }
            stopClass = stopClass ?? typeof(object); // If the stop class is present, use root
            if (typeInfo.BaseType == stopClass) return typeInfo.DeclaredProperties;
            return typeInfo.DeclaredProperties.Union(GetAllProperties(typeInfo.BaseType.GetTypeInfo(), stopClass));
        }

        public static PropertyInfo GetNamedProperty(Type type, string propertyName)
        {
            return GetNamedProperty(type.GetTypeInfo(), propertyName);
        }

        public static PropertyInfo GetNamedProperty(this TypeInfo typeInfo,
                string propertyName)
        {
            if (typeInfo == null) { return null; }
            var prop = typeInfo.DeclaredProperties.Where(x => x.Name == propertyName).FirstOrDefault();
            if (prop != null || typeInfo.BaseType == typeof(object)) return prop;
            return GetNamedProperty(typeInfo.BaseType.GetTypeInfo(), propertyName);
        }

        public static object GetPropertyValue(object input,
            string propertyName, bool throwOnMissing = false)
        {
            if (input == null)
            {
                if (throwOnMissing)
                {
                    throw new InvalidOperationException(
                               string.Format(CultureInfo.InvariantCulture, "Property {0} can't be determined for null", propertyName));
                }
                return null;
            }
            var typeInfo = input.GetType().GetTypeInfo();
            var prop = GetNamedProperty(typeInfo, propertyName);
            if (prop == null)
            {
                if (throwOnMissing)
                {
                    throw new InvalidOperationException(
                                string.Format(CultureInfo.InvariantCulture, "Property {0} not found", propertyName));
                }
                return null;
            }
            return prop.GetValue(input);
        }

        public static IEnumerable<CustomAttributeData> GetDefinedAttributes(TypeInfo typeInfo)
        {
            if (typeInfo == null) { return null; }
            return typeInfo.CustomAttributes;
        }

        public static bool TrySetPropertyValue(
            object item,
            PropertyInfo prop, object value)
        {
            if (prop == null) return false;
            prop.SetValue(item, value);
            return true;
        }

        public static bool TrySetPropertyValue(
                object item, 
                string propertyName,
                object value)
        {
            if (item == null) { return false; }
            var prop = ReflectionSupport.GetNamedProperty(item.GetType(), propertyName);
            return TrySetPropertyValue(item, prop, value);
        }
    }
}
