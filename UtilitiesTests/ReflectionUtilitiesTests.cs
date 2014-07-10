using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using RoslynDom.Common;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class ReflectionUtilitiesTests
    {
        private const string ReflectionUtilitiesCategory = "ReflectionUtilities";

        private class A
        {
            public string Foo { get { return "42"; } }
            public string Bar { get; set; }
            public string FooBar { set { } }
            public string Test<T>()
            { return typeof(T).Name; }

            public string TestFoo() { return ""; }

            public int TestFoo2(int x) { return x; }
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        public void Can_determine_if_property_can_be_read()
        {
            var obj = new A();
            Assert.IsTrue(ReflectionUtilities.CanGetProperty(obj, "Foo"));
            Assert.IsTrue(ReflectionUtilities.CanGetProperty(obj, "Bar"));
            Assert.IsFalse(ReflectionUtilities.CanGetProperty(obj, "FooBar"));
            Assert.IsFalse(ReflectionUtilities.CanGetProperty(obj, "FooBarX"));
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanGetProperty_throws_on_null()
        {
            Assert.AreEqual("", ReflectionUtilities.CanGetProperty(null, "Bar"));
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        public void Can_determine_if_property_can_be_written()
        {
            var obj = new A();
            Assert.IsFalse(ReflectionUtilities.CanSetProperty(obj, "Foo"));
            Assert.IsTrue(ReflectionUtilities.CanSetProperty(obj, "Bar"));
            Assert.IsTrue(ReflectionUtilities.CanSetProperty(obj, "FooBar"));
            Assert.IsFalse(ReflectionUtilities.CanSetProperty(obj, "FooBarX"));
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CanSetProperty_throws_on_null()
        {
            Assert.AreEqual("", ReflectionUtilities.CanSetProperty(null, "Bar"));
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        public void Can_read_property()
        {
            var obj = new A();
            obj.Bar = "31";
            Assert.AreEqual("31", ReflectionUtilities.GetPropertyValue(obj, "Bar"));
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPropertyValue_throws_on_null()
        {
            Assert.AreEqual("", ReflectionUtilities.GetPropertyValue(null, "Bar"));
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        public void Can_make_generic_method()
        {
            var methodInfo = ReflectionUtilities.MakeGenericMethod(typeof(A), "Test", typeof(string));
            var instance = new A();
            Assert.AreEqual("String", methodInfo.Invoke(instance, null));
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        public void Can_find_method_without_parameters()
        {
            var methodInfo = ReflectionUtilities.FindMethod(typeof(A), "TestFoo");
            Assert.AreEqual("TestFoo", methodInfo.Name);
        }

        [TestMethod, TestCategory(ReflectionUtilitiesCategory)]
        public void Can_find_methodwith_parameters()
        {
            var methodInfo = ReflectionUtilities.FindMethod(typeof(A), "TestFoo2", typeof(int));
            Assert.AreEqual("TestFoo2", methodInfo.Name);
        }

    }
}
