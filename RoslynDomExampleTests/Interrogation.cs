using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom;
using System.Linq;

namespace RoslynDomExampleTests
{
    /// <summary>
    /// Summary description for Interrogation
    /// </summary>
    [TestClass]
    public class Interrogation
    {
        [TestMethod]
        public void Get_using_statements()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var usings = root.Usings.ToArray();
            Assert.AreEqual(3, usings.Count());
            Assert.AreEqual("System.Diagnostics.Tracing", usings[0].Name);
            Assert.AreEqual("System", usings[1].Name);
            Assert.AreEqual("System.Linq", usings[2].Name);
        }

        [TestMethod]
        public void Get_namespaces()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspaces = root.Namespaces.ToArray();
            Assert.AreEqual(2, nspaces.Count());
            // TODO: Rework namespaces to reflect nesting, not current syntax. Below illustrate problem
            Assert.AreEqual("testing", nspaces[0].Name);
            Assert.AreEqual("Namespace1", nspaces[1].Name);
            Assert.AreEqual("testing.Namespace1", nspaces[1].QualifiedName);
        }

        [TestMethod]
        public void Get_classes_in_namespaces()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Get_structs_in_namespace()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Get_interfaces_in_namespace()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Get_enums_in_namespace()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Get_methods_in_class()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Get_fields_in_class()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Get_properties_in_class()
        {
            Assert.Inconclusive();
        }
    }
}
