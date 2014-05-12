using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KadGen.Common;
using System.Reflection;
using System.Linq;
using System.Globalization;

namespace TestRoslyn
{
    [TestClass]
    public class TestReflectionSupport
    {
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        private class TestAttribute : Attribute
        {
            public TestAttribute(int value)
            { this.Value = value; }

            public int Value { get; private set; }
        }

        private class TestAttribute2 : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        [Serializable()]
        private class TestAttribute3 : Attribute
        {
        }

        private enum TestEnum
        {
            Red,
            [TestAttribute(42)]
            Blue,
            Yellow

        }

        private class A
        {
            public string Foo { get; set; }
            public void BaseInstanceMethod() { }
        }

        private class B : A
        {
            public string Bar { get; set; }
            public string TestInstanceMethod() { return "Hello"; }
            public static string TestStaticMethod() { return "Goodbye"; }
            public string TestInstanceMethodWithParams(int val) { return val.ToString(); }
            public void GenericMethod<T>() { }
        }

        private class C : B
        { }

        [TestMethod]
        public void HasAttribute_should_return_true_for_attribute_found()
        {
            var type = typeof(TestAttribute);
            Assert.IsTrue(type.HasAttribute(typeof(AttributeUsageAttribute)));
        }

        [TestMethod]
        public void HasAttribute_should_return_false_for_attribute_not_found()
        {
            var type = typeof(TestAttribute);
            Assert.IsFalse(type.HasAttribute(typeof(SerializableAttribute)));
        }

        [TestMethod]
        public void HasAttribute_should_return_false_for_null_type()
        {
            var type = typeof(TestAttribute);
            Assert.IsFalse(ReflectionSupport .HasAttribute(null, typeof(SerializableAttribute)));
        }

        [TestMethod]
        public void HasAttribute_should_return_false_for_null_attributetype()
        {
            var type = typeof(TestAttribute);
            Assert.IsFalse(type.HasAttribute(null));
        }

        [TestMethod]
        public void GetAttributeForEnumValue_should_get_attribute_value()
        {
            var x = ReflectionSupport.GetAttributeForEnumValue(
                typeof(TestEnum),
                typeof(TestAttribute), TestEnum.Blue);
            Assert.AreEqual(42, x);
        }

        [TestMethod]
        public void GetAttributeForEnumValue_should_return_null_if_enum_value_not_found()
        {
            var x = ReflectionSupport.GetAttributeForEnumValue(
                typeof(TestEnum),
                typeof(TestAttribute), (TestEnum)42);
            Assert.IsNull(x);
        }

        [TestMethod]
        public void GetAttributeForEnumValue_should_return_null_if_enum_value_doesnt_contain_attribute()
        {
            var x = ReflectionSupport.GetAttributeForEnumValue(
                typeof(TestEnum),
                typeof(TestAttribute), TestEnum.Red);
            Assert.IsNull(x);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAttributeForEnumValue_should_throw_if_not_enum_type()
        {
            var x = ReflectionSupport.GetAttributeForEnumValue(
                typeof(int),
                typeof(TestAttribute), TestEnum.Blue);
        }

        [TestMethod]
        public void GetAttributeForEnumValue_should_return_null_if_null_passed()
        {
            var x = ReflectionSupport.GetAttributeForEnumValue(
                null,
                typeof(TestAttribute), TestEnum.Blue);
            var y = ReflectionSupport.GetAttributeForEnumValue(
               typeof(int),
               null, TestEnum.Blue);
            Assert.IsNull(x);
            Assert.IsNull(y);
        }
        [TestMethod]
        public void GetDefinedAttributes_should_return_correct_values()
        {
            var typeInfo = typeof(TestAttribute).GetTypeInfo();
            var attrs = ReflectionSupport.GetDefinedAttributes(typeInfo);
            Assert.AreEqual(1, attrs.Count());
        }

        [TestMethod]
        public void GetDefinedAttributes_should_not_fail_if_no_attributes()
        {
            var typeInfo = typeof(TestAttribute2).GetTypeInfo();
            var attrs = ReflectionSupport.GetDefinedAttributes(typeInfo);
            Assert.AreEqual(0, attrs.Count());
        }

        [TestMethod]
        public void GetDefinedAttributes_should_handle_multiple_attributes()
        {
            var typeInfo = typeof(TestAttribute3).GetTypeInfo();
            var attrs = ReflectionSupport.GetDefinedAttributes(typeInfo);
            Assert.AreEqual(2, attrs.Count());
        }

        [TestMethod]
        public void GetDefinedAttributes_should_return_null_for_null_typeinfo()
        {
            var attrs = ReflectionSupport.GetDefinedAttributes(null);
            Assert.IsNull(attrs);
        }

        [TestMethod]
        public void GetAllProperties_should_include_base_and_derived_properties()
        {
            var typeInfo = typeof(B).GetTypeInfo();
            var props = ReflectionSupport.GetAllProperties(typeInfo);
            Assert.AreEqual(2, props.Count());
        }

        [TestMethod]
        public void GetAllProperties_respects_stop_class()
        {
            var typeInfo = typeof(B).GetTypeInfo();
            var props = ReflectionSupport.GetAllProperties(typeInfo, typeof(A));
            Assert.AreEqual(1, props.Count());
        }

        [TestMethod]
        public void GetAllProperties_returns_nul_for_null_type_info()
        {
            var props = ReflectionSupport.GetAllProperties(null, typeof(A));
            Assert.IsNull(props);
        }

        [TestMethod]
        public void TrySetPropertyValue_should_set_value_and_return_true_if_found()
        {
            var item = new A();
            Assert.AreNotEqual("Test", item.Foo);
            var ret = ReflectionSupport.TrySetPropertyValue(item, "Foo", "Test");
            Assert.IsTrue(ret);
            Assert.AreEqual("Test", item.Foo);
        }

        [TestMethod]
        public void TrySetPropertyValue_should_not_set_value_and_return_false_if_not_found()
        {
            var item = new A();
            var ret = ReflectionSupport.TrySetPropertyValue(item, "Huh", "Test");
            Assert.IsFalse(ret);
            Assert.AreNotEqual("Test", item.Foo);
        }

        [TestMethod]
        public void TrySetPropertyValue_should_return_false_if_null_passed()
        {
            var ret = ReflectionSupport.TrySetPropertyValue(null, "Huh", "Test");
            Assert.IsFalse(ret);
        }
        [TestMethod]
        public void GetNamedProperty_should_return_prop_info_if_found_in_declared_class()
        {
            var item = new B();
            var prop = ReflectionSupport.GetNamedProperty(item.GetType().GetTypeInfo(), "Bar");
            Assert.IsNotNull(prop);
        }

        [TestMethod]
        public void GetNamedProperty_should_return_prop_info_if_found_in_base_class()
        {
            var item = new B();
            var prop = ReflectionSupport.GetNamedProperty(item.GetType().GetTypeInfo(), "Foo");
            Assert.IsNotNull(prop);
        }

        [TestMethod]
        public void GetNamedProperty_should_return_null_if_not_found()
        {
            var item = new B();
            var prop = ReflectionSupport.GetNamedProperty(item.GetType().GetTypeInfo(), "Huh");
            Assert.IsNull(prop);
        }

        [TestMethod]
        public void GetNamedProperty_should_return_null_for_empty_property_name()
        {
            var item = new B();
            var prop = ReflectionSupport
                .GetNamedProperty(item.GetType().GetTypeInfo(), "");
            Assert.IsNull(prop);
        }

        [TestMethod]
        public void GetNamedProperty_should_return_null_for_null_property_name()
        {
            var item = new B();
            var prop = ReflectionSupport
                .GetNamedProperty(item.GetType().GetTypeInfo(), null);
            Assert.IsNull(prop);
        }

        [TestMethod]
        public void GetNamedProperty_should_return_null_for_null_typeinfo()
        {
            var prop = ReflectionSupport.GetNamedProperty(null, "Huh");
            Assert.IsNull(prop);
        }

        [TestMethod]
        public void GetPropertyValue_should_return_correct_value_for_base()
        {
            var item = new B();
            item.Foo = "Hello";
            var result1 = ReflectionSupport.GetPropertyValue(item, "Foo");
            Assert.AreEqual("Hello", result1);
        }

        [TestMethod]
        public void GetPropertyValue_should_return_correct_value()
        {
            var item = new B();
            item.Bar = "World";
            var result1 = ReflectionSupport.GetPropertyValue(item, "Bar");
            Assert.AreEqual("World", result1);
        }
        [TestMethod]
        public void GetPropertyValue_should_return_null_for_null_input_unless_throwing()
        {
            var prop = ReflectionSupport.GetPropertyValue(null, "Huh");
            Assert.IsNull(prop);
        }

        [TestMethod]
        public void GetPropertyValue_should_return_null_for_empty_property_name_unless_throwing()
        {
            var item = new B();
            var prop = ReflectionSupport.GetPropertyValue(item, "");
            Assert.IsNull(prop);
        }

        [TestMethod]
        public void GetPropertyValue_should_return_null_for_null_property_name_unless_throwing()
        {
            var item = new B();
            var prop = ReflectionSupport.GetPropertyValue(item, null);
            Assert.IsNull(prop);
        }

        [TestMethod]
        public void GetPropertyValue_should_return_null_for_not_found_property_name_unless_throwing()
        {
            var item = new B();
            var prop = ReflectionSupport.GetPropertyValue(item, "Huh");
            Assert.IsNull(prop);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPropertyValue_should_throw_for_null_input_if_throwing()
        {
            var prop = ReflectionSupport.GetPropertyValue(null, "Huh", true);
            Assert.IsNull(prop);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPropertyValue_should_throw_for_empty_property_name_if_throwing()
        {
            var item = new B();
            var prop = ReflectionSupport.GetPropertyValue(item, "", true);
            Assert.IsNull(prop);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPropertyValue_should_throw_for_null_property_name_if_throwing()
        {
            var item = new B();
            var prop = ReflectionSupport.GetPropertyValue(item, null, true);
            Assert.IsNull(prop);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPropertyValue_should_throw_for_not_found_property_name_if_throwing()
        {
            var item = new B();
            var prop = ReflectionSupport.GetPropertyValue(item, "Huh", true);
            Assert.IsNull(prop);
        }




        [TestMethod]
        public void CultureSpecificToString_should_return_culture_specific_integer()
        {
            var val = ReflectionSupport.CultureSpecificToString(
                        typeof(DateTime), new DateTime(2013, 6, 7), CultureInfo.GetCultureInfo("FR-fr"));
            Assert.AreEqual("07/06/2013 00:00:00", val);
        }

        [TestMethod]
        public void CultureSpecificToString_should_return_null_for_null_type()
        {
            var val = ReflectionSupport.CultureSpecificToString(
                        null, new DateTime(2013, 6, 7), CultureInfo.GetCultureInfo("FR-fr"));
            Assert.IsNull( val);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException ))]
        public void CultureSpecificToString_should_throw_if_no_culture_specific_overload()
        {
            var val = ReflectionSupport.CultureSpecificToString(
                        typeof(AggregateException ), new DateTime(2013, 6, 7), CultureInfo.GetCultureInfo("FR-fr"));
            Assert.IsNull(val);
        }

        [TestMethod]
        public void GetMethod_should_find_method_in_declared_class()
        {
            var method = ReflectionSupport.GetMethod(typeof(B), "TestInstanceMethod");
            Assert.IsNotNull(method);
        }

        [TestMethod]
        public void GetMethod_should_not_find_method_in_base_class()
        {
            var method = ReflectionSupport.GetMethod(typeof(B), "BaseInstanceMethod");
            Assert.IsNull(method);
        }

        [TestMethod]
        public void GetResultFromInstanceMethod_should_get_correct_value()
        {
            var b = new B();
            var val = ReflectionSupport.GetResultFromInstanceMethod("TestInstanceMethod", b);
            Assert.AreEqual("Hello", val);
        }

        [TestMethod]
        public void GetResultFromInstanceMethod_should_return_null_for_null_instance()
        {
            var val = ReflectionSupport.GetResultFromInstanceMethod("TestInstanceMethod", null);
            Assert.IsNull( val);
        }

        [TestMethod]
        public void GetResultFromStaticMethod_should_get_correct_value_with_method_name()
        {
            var val = ReflectionSupport.GetResultFromStaticMethod(typeof(B), "TestStaticMethod");
            Assert.AreEqual("Goodbye", val);
        }

        [TestMethod]
        public void GetResultFromStaticMethod_should_get_correct_value_with_method_info()
        {
            var type = typeof(B);
            var methodName = "TestStaticMethod";
            var method = ReflectionSupport.GetMethod(type, methodName);
            var val = ReflectionSupport.GetResultFromStaticMethod(method);
            Assert.AreEqual("Goodbye", val);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetResultFromMethod_throws_if_method_null()
        {
            var val = ReflectionSupport.GetResultFromMethod(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetResultFromMethod_throws_if_parameters_bad()
        {
            var method = ReflectionSupport.GetMethod(typeof(int), "Parse", typeof(string));
            var val = ReflectionSupport.GetResultFromMethod(method, null, "3+4");
        }

        [TestMethod]
        public void GetResultFromMethod_returns_correct_value_no_parameters()
        {
            var b = new B();
            var method = ReflectionSupport.GetMethod(typeof(B), "TestInstanceMethod");
            var val = ReflectionSupport.GetResultFromMethod(method, b);
            Assert.AreEqual("Hello", val);
        }

        [TestMethod]
        public void GetResultFromMethod_returns_correct_value_with_parameters()
        {
            var b = new B();
            var method = ReflectionSupport.GetMethod(typeof(B), "TestInstanceMethodWithParams", typeof(Int32));
            var val = ReflectionSupport.GetResultFromMethod(method, b, 42);
            Assert.AreEqual("42", val);
        }

        [TestMethod]
        public void MakeMethod_creates_generic_type()
        {
            var method = ReflectionSupport.MakeMethod(
                typeof(B), "GenericMethod", typeof(int));
            Assert.IsNotNull(method);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeMethod_fails_if_not_generic_method()
        {
            var method = ReflectionSupport.MakeMethod(
                typeof(B), "TestInstanceMethod", typeof(int));
        }

        [TestMethod]
        public void TrySetPropertyValue_sets_value_with_propertyInfo()
        {
            var b = new B();
            var prop = ReflectionSupport.GetNamedProperty(typeof(B), "Bar");
            ReflectionSupport.TrySetPropertyValue(b, prop, "Test32");
            Assert.AreEqual("Test32", b.Bar);
        }

        [TestMethod]
        public void TrySetPropertyValue_sets_value_with_name()
        {
            var b = new B();
            var val = ReflectionSupport.TrySetPropertyValue(b, "Bar", "Test32");
            Assert.AreEqual("Test32", b.Bar);
            Assert.IsTrue(val);
        }

        [TestMethod]
        public void TrySetPropertyValue_returns_false_if_propertyInfo_is_null()
        {
            var b = new B();
            var val = ReflectionSupport.TrySetPropertyValue( b,"Bax", "Test32");
            Assert.IsFalse(val);
        }
    }
}
