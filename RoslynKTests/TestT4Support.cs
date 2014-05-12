using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KadGen.Common;

namespace TestRoslyn
{
    [TestClass]
    public class TestT4Support
    {
        [TestMethod]
        public void GetAttributeConstructor_should_return_correct_string_for_one_property()
        {
            var attr = new KadAttribute("Foo");
            attr.Properties.Add( new KadAttributeProperty("Bar", 16));
            var result = RuntimeT4Base.GetAttributeConstructor(attr);
            Assert.AreEqual("Foo(Bar = 16)", result);
        }

        [TestMethod]
        public void GetAttributeConstructor_should_return_correct_string_for_no_properties()
        {
            var attr = new KadAttribute("Foo");
            var result = RuntimeT4Base.GetAttributeConstructor(attr);
            Assert.AreEqual("Foo()", result);
        }

        [TestMethod]
        public void GetAttributeConstructor_should_return_correct_string_for_multiple_property_all_ordinal()
        {
            var attr = new KadAttribute("Foo");
            attr.Properties.Add(new KadAttributeProperty(2, 16));
            attr.Properties.Add(new KadAttributeProperty(6, "\"Bar\""));
            attr.Properties.Add(new KadAttributeProperty(3, 42));
            var result = RuntimeT4Base.GetAttributeConstructor(attr);
            Assert.AreEqual("Foo(16, 42, \"Bar\")", result);
        }
        [TestMethod]
        public void GetAttributeConstructor_should_return_correct_string_for_multiple_property_all_named()
        {
            var attr = new KadAttribute("Foo");
            attr.Properties.Add(new KadAttributeProperty("Bar1", 16));
            attr.Properties.Add(new KadAttributeProperty("Bar2", "\"Bar\""));
            attr.Properties.Add(new KadAttributeProperty("Bar3", 42));
            var result = RuntimeT4Base.GetAttributeConstructor(attr);
            Assert.AreEqual("Foo(Bar1 = 16, Bar2 = \"Bar\", Bar3 = 42)", result);
        }

        [TestMethod]
        public void GetAttributeConstructor_should_return_correct_string_for_multiple_property_mixed_named_first()
        {
            var attr = new KadAttribute("Foo");
            attr.Properties.Add(new KadAttributeProperty("Bar1", 16));
            attr.Properties.Add(new KadAttributeProperty("Bar2", "\"Bar\""));
            attr.Properties.Add(new KadAttributeProperty(3, 42));
            var result = RuntimeT4Base.GetAttributeConstructor(attr);
            Assert.AreEqual("Foo(42, Bar1 = 16, Bar2 = \"Bar\")", result);
        }

        [TestMethod]
        public void GetAttributeConstructor_should_return_correct_string_for_multiple_property_mixed_ordinal_first()
        {
            var attr = new KadAttribute("Foo");
            attr.Properties.Add(new KadAttributeProperty(3, 42));
            attr.Properties.Add(new KadAttributeProperty("Bar1", 16));
            attr.Properties.Add(new KadAttributeProperty("Bar2", "\"Bar\""));
            var result = RuntimeT4Base.GetAttributeConstructor(attr);
            Assert.AreEqual("Foo(42, Bar1 = 16, Bar2 = \"Bar\")", result);
        }
    }
}
