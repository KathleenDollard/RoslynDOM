using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KadGen.Common.Roslyn;
using KadGen.KadEventSource;
using System.Linq;
using System.Diagnostics.Tracing;

namespace TestRoslyn
{
    [TestClass]
    public class TestRoslynParserForMetadata
    {
        [TestMethod]
        public void Can_load_KadEvenSource_from_attributes()
        {
            var tree = @"
using KadMan.Common;

namespace ETWPreMan
{
    [ LocalizationResources(""Fred"") ]
    [ Name(""KadGen-Test-Temp"")]
    public class NormalEventSource
    {
        [Version(2)]
        [Keyword(3)]
        public void foo(int Bar, string Bar2)      {        }

        [Version(2)]
        [Keyword(3)]
        public void foo2()      {        }
    }
}";
            var parser = new KadGen.Common.Roslyn.ParserForMetadata<KadEventSource>();
            var result = parser.Parse(tree).First();
            Assert.AreEqual("KadGen-Test-Temp", result.Name);
            Assert.AreEqual("Fred", result.LocalizationResources);
            Assert.AreEqual("NormalEventSource", result.ClassName);
            Assert.AreEqual("ETWPreMan", result.NamespaceName);

            Assert.AreEqual(2, result.Events.Count());
            Assert.AreEqual("foo", result.Events.First().Name);
            Assert.AreEqual("foo2", result.Events.Last().Name);
            //Assert.AreEqual("foo", result.Events.First().CustomKeywords);
            Assert.AreEqual(null, result.Events.First().CustomOpcode);
            Assert.AreEqual(null, result.Events.First().CustomTask);
            Assert.AreEqual(0, result.Events.First().EventId);
            //Assert.AreEqual(EventKeywords.None, result.Events.First().Keywords);
            //Assert.AreEqual(EventLevel.Informational , result.Events.First().Level);
            //Assert.AreEqual("foo", result.Events.First().OpCode);

            Assert.AreEqual(2, result.Events.First().Parameters.Count());
            Assert.AreEqual(0, result.Events.Last().Parameters.Count());
            Assert.AreEqual("Bar", result.Events.First().Parameters.First().Name);
            Assert.AreEqual("Bar2", result.Events.First().Parameters.Last().Name);
            Assert.AreEqual("int", result.Events.First().Parameters.First().TypeName);
            Assert.AreEqual("string", result.Events.First().Parameters.Last().TypeName);
        }

        [TestMethod]
        public void Validation_should_fail_for_duplicate_event_ids()
        {
            var tree = @"
using KadMan.Common;

namespace ETWPreMan
{
    [ Name(""KadGen-Test-Temp2"")]
    public class NormalEventSource
    {
        [EventId(2)]
        public void foo(int Bar, string Bar2)      {        }

        public void foo2()      {        }
    }
}";
            var parser = new KadGen.Common.Roslyn.ParserForMetadata<KadEventSource>();
            var result = parser.Parse(tree).First();
            Assert.IsFalse(result.ValidateAndUpdate());
        }

        [TestMethod]
        public void Validation_should_fail_for_event_id_lessthan_one()
        {
            var tree = @"
using KadMan.Common;

namespace ETWPreMan
{
    [ Name(""KadGen-Test-Temp2"")]
    public class NormalEventSource
    {
        [EventId(-1)]
        public void foo(int Bar, string Bar2)      {        }

        public void foo2()      {        }
    }
}";
            var parser = new KadGen.Common.Roslyn.ParserForMetadata<KadEventSource>();
            var result = parser.Parse(tree).First();
            Assert.IsFalse(result.ValidateAndUpdate());
        }
    }
}
