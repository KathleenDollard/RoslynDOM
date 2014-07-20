using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomExampleTests
{
    [TestClass]
    public class Walkthroughs
    {
        private string fileName = "Walkthrough_1_code.cs";
        private string outputFileName = "Walkthrough_1_code_test.cs";

        [TestMethod]
        public void Walkthrogh_1_Load_and_check_code()
        {
            var factory = RDomCSharpFactory.Factory;
            var root = factory.GetRootFromFile(fileName);
            var output = factory.BuildSyntax(root).ToString();
            File.WriteAllText(outputFileName, output);
        }

        [TestMethod]
        public void Walkthrogh_2_2_Navigate_and_interrogate_code()
        {
            var factory = RDomCSharpFactory.Factory;
            var root = factory.GetRootFromFile(fileName);
            Assert.AreEqual(1, root.Usings.Count());
            Assert.AreEqual("System", root.Usings.First().Name);
            Assert.AreEqual(1, root.Namespaces.Count());
            Assert.AreEqual(1, root.RootClasses.Count());
            var methods = root.RootClasses.First().Methods.ToArray();
            Assert.AreEqual(0, methods[0].Parameters.Count());
            Assert.AreEqual(1, methods[1].Parameters.Count());
            Assert.AreEqual("dummy", methods[1].Parameters.First().Name);
        }

        public string FooBar
        {
            get
            {
                ushort z = 432;
                return z.ToString();
            }
        }

        [TestMethod]
        public void Walkthrogh_2_3_Navigate_and_interrogate_code()
        {
            var factory = RDomCSharpFactory.Factory;
            var root = factory.GetRootFromFile(fileName);

            // Explore variables that have any uint type
            var uintVars = root
                .Descendants.OfType<IVariable>()
                .Where(x => x.Type.Name.StartsWith("UInt"))
                .ToArray();
            Assert.AreEqual(3, uintVars.Count());
            Assert.AreEqual("y", uintVars[0].Name);
            Assert.AreEqual("x", uintVars[1].Name);
            Assert.AreEqual("z", uintVars[2].Name);

            // Retrieve methods and properties with Uint types
            // Explore variables that have any uint type
            var uintCode = (from cl in root.Descendants.OfType<IStatementContainer>()
                            from v in cl.Descendants.OfType<IVariable>()
                            where v.Type.Name.StartsWith("UInt")
                            select new
                            {
                                containerName = cl.Name,
                                variableName = v.Name
                            })
                           .ToArray();
            Assert.AreEqual("Foo", uintCode[0].containerName);
            Assert.AreEqual("y", uintCode[0].variableName);

            Assert.AreEqual("Foo2", uintCode[1].containerName);
            Assert.AreEqual("x", uintCode[1].variableName);

            Assert.AreEqual("get_FooBar", uintCode[2].containerName);
            Assert.AreEqual("z", uintCode[2].variableName);

        }
    }
}
