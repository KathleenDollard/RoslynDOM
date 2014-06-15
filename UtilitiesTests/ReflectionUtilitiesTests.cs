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
        private class A
        {
            public string Foo { get { return "42"; } }
            public string Bar { get; set; }
            public string FooBar { set { } }
        }

        [TestMethod]
        public void Can_determine_if_property_can_be_read()
        {
            var obj = new A();
            Assert.IsTrue(ReflectionUtilities.CanGetProperty(obj, "Foo"));
            Assert.IsTrue(ReflectionUtilities.CanGetProperty(obj, "Bar"));
            Assert.IsFalse(ReflectionUtilities.CanGetProperty(obj, "FooBar"));
        }

        [TestMethod]
        public void Can_determine_if_property_can_be_written()
        {
            var obj = new A();
            Assert.IsFalse(ReflectionUtilities. CanSetProperty(obj, "Foo"));
            Assert.IsTrue(ReflectionUtilities. CanSetProperty(obj, "Bar"));
            Assert.IsTrue(ReflectionUtilities.CanSetProperty(obj, "FooBar"));
        }

        [TestMethod]
        public void Can_read_property()
        {
            var obj = new A();
            obj.Bar = "31";
            Assert.AreEqual("31", ReflectionUtilities.GetPropertyValue(obj, "Bar"));
        }


    }
}
