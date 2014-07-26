using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class ContainerTests
    {
        private const string StructureNestingCategory = "StructureNesting";
        private const string ClassNestingCategory = "ClassNesting";

        #region structure nesting tests
        [TestMethod]
        [TestCategory(StructureNestingCategory)]
        public void Can_get_classes_for_structure()
        {
            var csharpCode = @"
                        public struct Foo
                        {
                        public class Bar1{};
                        private class Bar2{};
                        internal class Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var structure = root.Structures.First();
            Assert.AreEqual(3, structure.Classes.Count());
            Assert.AreEqual(0, structure.Structures.Count());
            Assert.AreEqual(0, structure.Interfaces.Count());
            Assert.AreEqual(0, structure.Enums.Count());
            Assert.AreEqual(3, structure.Types.Count());
        }

        [TestMethod]
        [TestCategory(StructureNestingCategory)]
        public void Can_get_structs_for_structure()
        {
            var csharpCode = @"
                        public struct Foo
                        {
                        public struct Bar1{};
                        private struct Bar2{};
                        internal struct Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var structure = root.Structures.First();
            Assert.AreEqual(0, structure.Classes.Count());
            Assert.AreEqual(3, structure.Structures.Count());
            Assert.AreEqual(0, structure.Interfaces.Count());
            Assert.AreEqual(0, structure.Enums.Count());
            Assert.AreEqual(3, structure.Types.Count());
        }


        [TestMethod]
        [TestCategory(StructureNestingCategory)]
        public void Can_get_interfacess_for_structure()
        {
            var csharpCode = @"
                        public struct Foo
                        {
                        public interface Bar1{};
                        private interface Bar2{};
                        internal interface Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var structure = root.Structures.First();
            Assert.AreEqual(0, structure.Classes.Count());
            Assert.AreEqual(0, structure.Structures.Count());
            Assert.AreEqual(3, structure.Interfaces.Count());
            Assert.AreEqual(0, structure.Enums.Count());
            Assert.AreEqual(3, structure.Types.Count());
        }


        [TestMethod]
        [TestCategory(StructureNestingCategory)]
        public void Can_get_enums_for_structure()
        {
            var csharpCode = @"
                        public struct Foo
                        {
                        public enum Bar1{};
                        private enum Bar2{};
                        internal enum Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var structure = root.Structures.First();
            Assert.AreEqual(0, structure.Classes.Count());
            Assert.AreEqual(0, structure.Structures.Count());
            Assert.AreEqual(0, structure.Interfaces.Count());
            Assert.AreEqual(3, structure.Enums.Count());
            Assert.AreEqual(3, structure.Types.Count());
        }

        [TestMethod]
        [TestCategory(StructureNestingCategory)]
        public void Can_get_types_for_structure()
        {
            var csharpCode = @"
                        public struct Foo
                        {
                        public class Bar1{};
                        private struct Bar2{};
                        internal interface Bar2{};
                        internal enum Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var structure = root.Structures.First();
            Assert.AreEqual(1, structure.Classes.Count());
            Assert.AreEqual(1, structure.Structures.Count());
            Assert.AreEqual(1, structure.Interfaces.Count());
            Assert.AreEqual(1, structure.Enums.Count());
            Assert.AreEqual(4, structure.Types.Count());

        }

        #endregion

        #region class nesting tests
        [TestMethod]
        [TestCategory(ClassNestingCategory)]
        public void Can_get_classes_for_class()
        {
            var csharpCode = @"
                        public class Foo
                        {
                        public class Bar1{};
                        private class Bar2{};
                        internal class Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.Classes.First();
            Assert.AreEqual(3, cl.Classes.Count());
            Assert.AreEqual(0, cl.Structures.Count());
            Assert.AreEqual(0, cl.Interfaces.Count());
            Assert.AreEqual(0, cl.Enums.Count());
            Assert.AreEqual(3, cl.Types.Count());
        }

        [TestMethod]
        [TestCategory(ClassNestingCategory)]
        public void Can_get_structs_for_class()
        {
            var csharpCode = @"
                        public class Foo
                        {
                        public struct Bar1{};
                        private struct Bar2{};
                        internal struct Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.Classes.First();
            Assert.AreEqual(0, cl.Classes.Count());
            Assert.AreEqual(3, cl.Structures.Count());
            Assert.AreEqual(0, cl.Interfaces.Count());
            Assert.AreEqual(0, cl.Enums.Count());
            Assert.AreEqual(3, cl.Types.Count());
        }


        [TestMethod]
        [TestCategory(ClassNestingCategory)]
        public void Can_get_interfacess_for_class()
        {
            var csharpCode = @"
                        public class Foo
                        {
                        public interface Bar1{};
                        private interface Bar2{};
                        internal interface Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.Classes.First();
            Assert.AreEqual(0, cl.Classes.Count());
            Assert.AreEqual(0, cl.Structures.Count());
            Assert.AreEqual(3, cl.Interfaces.Count());
            Assert.AreEqual(0, cl.Enums.Count());
            Assert.AreEqual(3, cl.Types.Count());
        }


        [TestMethod]
        [TestCategory(ClassNestingCategory)]
        public void Can_get_enums_for_class()
        {
            var csharpCode = @"
                        public class Foo
                        {
                        public enum Bar1{};
                        private enum Bar2{};
                        internal enum Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.Classes.First();
            Assert.AreEqual(0, cl.Classes.Count());
            Assert.AreEqual(0, cl.Structures.Count());
            Assert.AreEqual(0, cl.Interfaces.Count());
            Assert.AreEqual(3, cl.Enums.Count());
            Assert.AreEqual(3, cl.Types.Count());
        }

        [TestMethod]
        [TestCategory(ClassNestingCategory)]
        public void Can_get_types_for_class()
        {
            var csharpCode = @"
                        public class Foo
                        {
                        public class Bar1{};
                        private struct Bar2{};
                        internal interface Bar2{};
                        internal enum Bar2{};
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.Classes.First();
            Assert.AreEqual(1, cl.Classes.Count());
            Assert.AreEqual(1, cl.Structures.Count());
            Assert.AreEqual(1, cl.Interfaces.Count());
            Assert.AreEqual(1, cl.Enums.Count());
            Assert.AreEqual(4, cl.Types.Count());

        }

        #endregion
    }
}
