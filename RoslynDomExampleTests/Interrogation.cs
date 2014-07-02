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
            Assert.AreEqual(3, nspaces.Count());
            // TODO: Rework namespaces to reflect nesting, not current syntax. Below illustrate problem
            Assert.AreEqual("testing", nspaces[0].Name);
            Assert.AreEqual("Namespace1", nspaces[1].Name);
            Assert.AreEqual("testing.Namespace1", nspaces[1].QualifiedName);
            Assert.AreEqual("Namespace2", nspaces[2].Name);
        }

        [TestMethod]
        public void Get_all_namespaces()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            // All Namespaces are designed to return all creatable qualified syntax names, 
            // and is not anticipated to be very useful. Order of namespaces is linear out to in.
            var nspaces = root.AllChildNamespaces.ToArray();
            Assert.AreEqual(4, nspaces.Count());
            Assert.AreEqual("testing", nspaces[0].Name);
            Assert.AreEqual("Namespace3", nspaces[1].Name);
            Assert.AreEqual("testing.Namespace3", nspaces[1].QualifiedName);
            Assert.AreEqual("testing.Namespace1", nspaces[2].QualifiedName);
            Assert.AreEqual("Namespace2", nspaces[3].Name);
        }

        [TestMethod]
        public void Get_non_empty_namespaces()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            // Nonempty namespaces are anticipated to be the primary namespace access mechanism. 
            var nspaces = root.NonemptyNamespaces.ToArray();
            Assert.AreEqual(2, nspaces.Count());
            Assert.AreEqual("Namespace3", nspaces[0].Name);
            Assert.AreEqual("testing.Namespace3", nspaces[0].QualifiedName);
            Assert.AreEqual("Namespace2", nspaces[1].Name);
        }

        [TestMethod]
        public void Get_classes_in_namespaces()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var classes = nspace.Classes.ToArray();
            Assert.AreEqual(2, classes.Count());
            Assert.AreEqual("FooClass", classes[0].Name);
            Assert.AreEqual("BarClass", classes[1].Name);
        }

        [TestMethod]
        public void Get_structs_in_namespace()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var structures = nspace.Structures.ToArray();
            Assert.AreEqual(1, structures.Count());
            Assert.AreEqual("FooStruct", structures[0].Name);
        }

        [TestMethod]
        public void Get_interfaces_in_namespace()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var interfaces = nspace.Interfaces.ToArray();
            Assert.AreEqual(1, interfaces.Count());
            Assert.AreEqual("FooInterface", interfaces[0].Name);
    }

        [TestMethod]
        public void Get_enums_in_namespace()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var enums = nspace.Enums.ToArray();
            Assert.AreEqual(1, enums.Count());
            Assert.AreEqual("FooEnum", enums[0].Name);
        }

        [TestMethod]
        public void Get_methods_in_class()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var methods = nspace.Classes.First().Methods.ToArray();
            Assert.AreEqual(1, methods.Count());
            Assert.AreEqual("FooMethod", methods[0].Name);
        }

        [TestMethod]
        public void Get_fields_in_class()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var fields = nspace.Classes.First().Fields.ToArray();
            Assert.AreEqual(1, fields.Count());
            Assert.AreEqual("FooField", fields[0].Name);
        }

        [TestMethod]
        public void Get_properties_in_class()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var properties = nspace.Classes.First().Properties.ToArray();
            Assert.AreEqual(1, properties.Count());
            Assert.AreEqual("FooProperty", properties[0].Name);
        }

        [TestMethod]
        public void Get_parameters_for_method()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            var nspace = root.Namespaces.Last();
            var parameters = nspace.Classes.First().Methods.First().Parameters.ToArray() ;
            Assert.AreEqual(2, parameters.Count());
            Assert.AreEqual("bar1", parameters[0].Name);
            Assert.AreEqual("bar2", parameters[1].Name);
        }
    }
}
