using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;

namespace RoslynDomTests
{
    [TestClass]
   public class CharacteristicTests
    {
        private const string ReturnedTypeCategory = "ReturnedType";
        private const string AccessModifierCategory = "AccessModifier";

        #region returned type tests
        [TestMethod]
        [TestCategory(ReturnedTypeCategory)]
        public void Can_get_field_return_type()
        {
            var csharpCode = @"
                        public class Foo
{
public string Bar;
}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var field = root.Classes.First().Fields.First();
            var retType = field.ReturnType;
            Assert.IsNotNull(retType);
            Assert.AreEqual("String", retType.Name, "Name");
            Assert.AreEqual("System.String", retType.QualifiedName, "QualifiedName");
            Assert.AreEqual("String", retType.OuterName, "OuterName");
            Assert.AreEqual("System", retType.Namespace, "Namespace");
        }

        [TestMethod]
        [TestCategory(ReturnedTypeCategory)]
        public void Can_get_property_return_type()
        {
            var csharpCode = @"
                        public class Foo
{
public int Bar{get;};
}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var property = root.Classes.First().Properties.First();
            var retType = property.ReturnType;
            Assert.IsNotNull(retType);
            Assert.AreEqual("Int32", retType.Name, "Name");
            Assert.AreEqual("System.Int32", retType.QualifiedName, "QualifiedName");
            Assert.AreEqual("Int32", retType.OuterName, "OuterName");
            Assert.AreEqual("System", retType.Namespace, "Namespace");

        }

        [TestMethod]
        [TestCategory(ReturnedTypeCategory)]
        public void Can_get_method_return_type()
        {
            var csharpCode = @"
                        public class Foo
{
public string Bar() {};
}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var method = root.Classes.First().Methods.First();
            var retType = method.ReturnType;
            Assert.IsNotNull(retType);
            Assert.AreEqual("String", retType.Name, "Name");
            Assert.AreEqual("System.String", retType.QualifiedName, "QualifiedName");
            Assert.AreEqual("String", retType.OuterName, "OuterName");
            Assert.AreEqual("System", retType.Namespace, "Namespace");
        }

        public class A
        { }
        #endregion

#region access modifier tests
        [TestMethod]
        [TestCategory(AccessModifierCategory)]
        public void Can_get_access_modifier_for_class()
        {
            var csharpCode = @"
public class Foo1{}
private class Foo2{}
protected class Foo3{}
protected internal class Foo4{}
internal class Foo5{}
class Foo6{}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var classes = root.Classes.ToArray();
            Assert.AreEqual(AccessModifier.Public, classes[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, classes[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, classes[2].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, classes[3].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, classes[4].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, classes[5].AccessModifier);

        }

        [TestMethod]
        [TestCategory(AccessModifierCategory)]
        public void Can_get_access_modifier_for_Interface()
        {
            var csharpCode = @"
public interface Foo1{}
private interface Foo2{}
protected interface Foo3{}
protected internal interface Foo4{}
internal interface Foo5{}
interface Foo6{}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var interfaces = root.Interfaces.ToArray();
            Assert.AreEqual(AccessModifier.Public, interfaces[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, interfaces[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, interfaces[2].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, interfaces[3].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, interfaces[4].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, interfaces[5].AccessModifier);

        }

        [TestMethod]
        [TestCategory(AccessModifierCategory)]
        public void Can_get_access_modifier_for_Structure()
        {
            var csharpCode = @"
public struct Foo1{}
private struct Foo2{}
protected struct Foo3{}
protected internal struct Foo4{}
internal struct Foo5{}
struct Foo6{}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var structures = root.Structures.ToArray();
            Assert.AreEqual(AccessModifier.Public, structures[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, structures[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, structures[2].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, structures[3].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, structures[4].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, structures[5].AccessModifier);

        }

        [TestMethod]
        [TestCategory(AccessModifierCategory)]
        public void Can_get_access_modifier_for_enum()
        {
            var csharpCode = @"
public enum Foo1{}
private enum Foo2{}
protected enum Foo3{}
protected internal enum Foo4{}
internal enum Foo5{}
enum Foo6{}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var enums = root.Enums.ToArray();
            Assert.AreEqual(AccessModifier.Public, enums[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, enums[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, enums[2].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, enums[3].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, enums[4].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, enums[5].AccessModifier);

        }

        [TestMethod]
        [TestCategory(AccessModifierCategory)]
        public void Can_get_access_modifier_for_members_in_class()
        {
            var csharpCode = @"
public class Foo1
{
    public string Bar1;
    public string Bar2 {get;};
    public string Bar3(){};
    private string Bar4;
    private string Bar5 {get;};
    private string Bar6(){};
    protected string Bar7;
    protected string Bar8 {get;};
    protected string Bar9(){};
    protected internal string Bar10;
    protected internal string Bar11 {get;};
    protected internal string Bar12(){};
    internal string Bar13;
    internal string Bar14 {get;};
    internal string Bar15(){};
    string Bar16;
    string Bar17 {get;};
    string Bar18(){};
}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var members = root.Classes.First().Members.ToArray();
            Assert.AreEqual(AccessModifier.Public, members[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[2].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[3].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[4].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[5].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, members[6].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, members[7].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, members[8].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, members[9].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, members[10].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, members[11].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, members[12].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, members[13].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, members[14].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[15].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[16].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[17].AccessModifier);

        }

        [TestMethod]
        [TestCategory(AccessModifierCategory)]
        public void Can_get_access_modifier_for_members_in_structure()
        {
            var csharpCode = @"
public struct Foo1
{
    public string Bar1;
    public string Bar2 {get;};
    public string Bar3(){};
    private string Bar4;
    private string Bar5 {get;};
    private string Bar6(){};
    protected string Bar7;
    protected string Bar8 {get;};
    protected string Bar9(){};
    protected internal string Bar10;
    protected internal string Bar11 {get;};
    protected internal string Bar12(){};
    internal string Bar13;
    internal string Bar14 {get;};
    internal string Bar15(){};
    string Bar16;
    string Bar17 {get;};
    string Bar18(){};
}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var members = root.Structures.First().Members.ToArray();
            Assert.AreEqual(AccessModifier.Public, members[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[2].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[3].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[4].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[5].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, members[6].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, members[7].AccessModifier);
            Assert.AreEqual(AccessModifier.Protected, members[8].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, members[9].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, members[10].AccessModifier);
            Assert.AreEqual(AccessModifier.ProtectedOrInternal, members[11].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, members[12].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, members[13].AccessModifier);
            Assert.AreEqual(AccessModifier.Internal, members[14].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[15].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[16].AccessModifier);
            Assert.AreEqual(AccessModifier.Private, members[17].AccessModifier);

        }

        [TestMethod]
        [TestCategory(AccessModifierCategory)]
        public void Can_get_access_modifier_for_members_in_interface()
        {
            var csharpCode = @"
public interface Foo1
{

    string Bar16;
    string Bar17 {get;};
    string Bar18(){};
}
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var members = root.Interfaces.First().Members.ToArray();
            Assert.AreEqual(AccessModifier.Public, members[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[2].AccessModifier);
        }
        #endregion 
    }
}
