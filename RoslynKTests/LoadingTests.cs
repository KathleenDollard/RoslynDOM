using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynK;
using System.Linq;

namespace RoslynKTests
{
    [TestClass]
    public class LoadingTests
    {
        [TestMethod]
        public void Should_load_simple_tree()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace kad_Template
{
}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            Assert.IsNotNull(treeWrapper);
            Assert.IsNotNull(treeWrapper.RawItem);
            Assert.IsInstanceOfType(treeWrapper.RawItem, typeof(SyntaxTree));
            Assert.IsNotNull(treeWrapper.Root);
            Assert.AreEqual("TreeWrapper", treeWrapper.Name);
        }

        [TestMethod]
        public void Should_load_basic_empty_tree()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
using System;
using System.Reflection;

namespace kad_Template
{
    using System;
    public static class kad_MetadataExtension<KadEventSource>
    {
    }
}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            var root = treeWrapper.Root;
            CheckRootCounts(root, usings: 2, members: 1, namespaces: 1, classesAndStructs: 0,
                classes: 0, structures: 0, interfaces: 0, enums: 0);
        }

        [TestMethod]
        public void Should_load_basic_weird_namespace_and_class_tree()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"

namespace Foo
{
}

public class B{}


namespace kad_Template
{
    using System;
    public static class kad_MetadataExtension<KadEventSource>
    {
    }
}

public class A {}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            var root = treeWrapper.Root;
            CheckRootCounts(root, usings: 0, members: 4, namespaces: 2, classesAndStructs: 2,
                classes: 2, structures: 0, interfaces: 0, enums: 0);
        }
        [TestMethod]
        public void Should_load_interfaces()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"

public interface Foo
{
}

namespace kad_Template
{
    public interface kad_MetadataExtension<KadEventSource> {}
    {
    }
}

public class A {}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            var root = treeWrapper.Root;
            CheckRootCounts(root, usings: 0, members: 3, namespaces: 1, classesAndStructs: 1,
                classes: 1, structures: 0, interfaces: 1, enums: 0);
        }

        [TestMethod]
        public void Should_load_structures()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
public struct Foo
{
}

namespace kad_Template
{
    public structure kad_MetadataExtension<KadEventSource> {}
    {
    }
}

public struct A {}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            var root = treeWrapper.Root;
            CheckRootCounts(root, usings: 0, members: 3, namespaces: 1, classesAndStructs: 2,
                classes: 0, structures: 2, interfaces: 0, enums: 0);
        }

        [TestMethod]
        public void Should_load_enums()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
    private enum colors
    {
        red = 0,
        green,
        blue
    }
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            var root = treeWrapper.Root;
            CheckRootCounts(root, usings: 0, members: 1, namespaces: 0, classesAndStructs: 0,
                classes: 0, structures: 0, interfaces: 0, enums: 1);
        }

        [TestMethod]
        public void Should_load_simple_namespace()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
using System;
using System.Reflection;

namespace kad_Template
{
    using System;
    public static class kad_MetadataExtension<KadEventSource>
    {
    }
}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            var root = treeWrapper.Root;
            var nspace = root.Namespaces.First();
            CheckStemContainerCounts(nspace, members: 1, namespaces: 1, classesAndStructs: 1,
                classes: 1, structures: 0, interfaces: 0, enums: 0);
        }

        [TestMethod]
        public void Should_load_simple_class()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace kad_Template
{
    public static class kad_MetadataExtension<KadEventSource>
    {
        private string _bar2;

        public void Foo(int i);
        public int Foo(string s, int i);
        public string Bar{ get; set; }
        public string Bar2
        {
            get
            {
                return _bar2;
            }
        }
    }
}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            var root = treeWrapper.Root;
            var nspace = root.Namespaces.First();
            var cls = nspace.Classes.First();
            CheckClassCounts(cls, members: 5, methods: 2, properties: 2, fields: 1,
                nestedTypes: 0, nestedClasses: 0, nestedStructures: 0, nestedInterfaces: 0, nestedEnums: 0);
        }


        private void CheckRootCounts(IRoot root,
                int usings, int members, int namespaces, int classesAndStructs,
                int classes, int structures, int interfaces, int enums)
        {
            var _usings = root.Usings;
            var _members = root.Members;
            var _namespaces = root.Namespaces;
            var _classesAndStructs = root.ClassesAndStructures;
            var _classes = root.Classes;
            var _structures = root.Structures;
            var _interfaces = root.Interfaces;
            var _enums = root.Enums;
            Assert.AreEqual(usings, _usings.Count(), "Usings wrong");
            Assert.AreEqual(members, _members.Count(), "Members wrong");
            Assert.AreEqual(namespaces, _namespaces.Count(), "Namespaces wrong");
            Assert.AreEqual(classesAndStructs, _classesAndStructs.Count(), "ClassesAndStructs wrong");
            Assert.AreEqual(classes, _classes.Count(), "Classes wrong");
            Assert.AreEqual(structures, _structures.Count(), "Structures wrong");
            Assert.AreEqual(interfaces, _interfaces.Count(), "Interfaces wrong");
            Assert.AreEqual(enums, _enums.Count(), "Enums wrong");

        }

        private void CheckStemContainerCounts(INamespace nspace,
          int members, int namespaces, int classesAndStructs,
         int classes, int structures, int interfaces, int enums)
        {
            var _members = nspace.Members;
            var _namespaces = nspace.Namespaces;
            var _classesAndStructs = nspace.ClassesAndStructures;
            var _classes = nspace.Classes;
            var _structures = nspace.Structures;
            var _interfaces = nspace.Interfaces;
            var _enums = nspace.Enums;
            Assert.AreEqual(members, _members.Count(), "Members wrong");
            Assert.AreEqual(classesAndStructs, _classesAndStructs.Count(), "ClassesAndStructs wrong");
            Assert.AreEqual(classes, _classes.Count(), "Classes wrong");
            Assert.AreEqual(structures, _structures.Count(), "Structures wrong");
            Assert.AreEqual(interfaces, _interfaces.Count(), "Interfaces wrong");
            Assert.AreEqual(enums, _enums.Count(), "Enums wrong");

        }

        private void CheckClassCounts(IClass item,
                   int members, int methods, int properties, int fields,
                   int nestedTypes, int nestedClasses, int nestedStructures, int nestedInterfaces, int nestedEnums)
        {
            CheckClassOrStructCounts(item, members, methods, properties, fields);
            var _types = item.Types;
            var _classes = item.Classes;
            var _structures = item.Structures;
            var _interfaces = item.Interfaces;
            var _enums = item.Enums;
            Assert.AreEqual(nestedTypes, _types.Count(), "Types wrong");
            Assert.AreEqual(nestedClasses, _classes.Count(), "Classes wrong");
            Assert.AreEqual(nestedStructures, _structures.Count(), "Structures wrong");
            Assert.AreEqual(nestedInterfaces, _interfaces.Count(), "Interfaces wrong");
            Assert.AreEqual(nestedEnums, _enums.Count(), "Enums wrong");

        }

        private void CheckClassOrStructCounts(IClassOrStruct item,
                 int members, int methods, int properties, int fields)
        {
            var _members = item.Members;
            var _methods = item.Methods;
            var _properties = item.Properties;
            var _fields = item.Fields;
            Assert.AreEqual(members, _members.Count(), "Members wrong");
            Assert.AreEqual(methods, _methods.Count(), "ClassesAndStructs wrong");
            Assert.AreEqual(properties, _properties.Count(), "Classes wrong");
            Assert.AreEqual(fields, _fields.Count(), "Structures wrong");

        }


    }
}
