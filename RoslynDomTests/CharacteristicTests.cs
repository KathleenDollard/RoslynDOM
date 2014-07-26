using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class CharacteristicTests
    {
        private const string ReturnedTypeCategory = "ReturnedType";
        private const string AccessModifierCategory = "AccessModifier";
        private const string VirtualCategory = "Virtual";
        private const string AbstractCategory = "Abstract";
        private const string SealedCategory = "Sealed";
        private const string OverrideCategory = "Override";
        private const string NewCategory = "New";
        private const string StaticCategory = "Static";
        private const string EnumsCategory = "Enums";
        private const string ImplementedInterfacesCategory = "ImplementedInterfaces";
        private const string BaseTypeCategory = "BaseType";
        private const string ParameterAndMethodCategory = "ParameterAndMethod";
        private const string ReturnTypeNameCategory = "ReturnTypeName";
        private const string NamespaceCategory = "Namespace";
        private const string MemberKindCategory = "MemberKind";
        private const string PropertyAccessCategory = "PropertyAccess";
        private const string MiscellaneousCategory = "Miscellaneous";

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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
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
            // This test is failing and I believe it to be due to a temporary CTP bug. So, I made it inconclusive
            // to avoid confusing people interested in the library release
            //Assert.Inconclusive();

            var csharpCode = @"
                        public class Foo
{
public Namespace1.A  Bar() {};
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var method = root.Classes.First().Methods.First();
            var retType = method.ReturnType;
            Assert.IsNotNull(retType);
            Assert.AreEqual("A", retType.Name, "Name");
            Assert.AreEqual("Namespace1.A", retType.QualifiedName, "QualifiedName");
            Assert.AreEqual("Namespace1.A", retType.OuterName, "OuterName");
        }

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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var enums = root.Enums.ToArray();
            Assert.AreEqual(6, enums.Count());
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var members = root.Interfaces.First().Members.ToArray();
            Assert.AreEqual(AccessModifier.Public, members[0].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[1].AccessModifier);
            Assert.AreEqual(AccessModifier.Public, members[2].AccessModifier);
        }
        #endregion

        #region virtual tests
        [TestMethod]
        [TestCategory(VirtualCategory)]
        public void Can_get_virtual_for_method()
        {
            var csharpCode = @"
public class Foo1
{
  public virtual string Bar1(){}
  public string Bar2(){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Methods.ToArray();
            Assert.IsTrue(methods[0].IsVirtual);
            Assert.IsFalse(methods[0].IsAbstract);
            Assert.IsFalse(methods[0].IsOverride);
            Assert.IsFalse(methods[0].IsSealed);
            Assert.IsFalse(methods[1].IsVirtual);
            Assert.IsFalse(methods[1].IsAbstract);
            Assert.IsFalse(methods[1].IsOverride);
            Assert.IsFalse(methods[1].IsSealed);

        }

        [TestMethod]
        [TestCategory(VirtualCategory)]
        public void Can_get_virtual_for_property()
        {
            var csharpCode = @"
public class Foo1
{
  public virtual string Bar1{get;}
  public string Bar2{get;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Properties.ToArray();
            Assert.IsTrue(methods[0].IsVirtual);
            Assert.IsFalse(methods[0].IsAbstract);
            Assert.IsFalse(methods[0].IsOverride);
            Assert.IsFalse(methods[0].IsSealed);
            Assert.IsFalse(methods[1].IsVirtual);
            Assert.IsFalse(methods[1].IsAbstract);
            Assert.IsFalse(methods[1].IsOverride);
            Assert.IsFalse(methods[1].IsSealed);

        }

        #endregion

        #region override tests
        [TestMethod]
        [TestCategory(OverrideCategory)]
        public void Can_get_override_for_method()
        {
            var csharpCode = @"
public class Foo1
{
  public override string Bar1(){}
  public string Bar2(){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var properties = root.Classes.First().Methods.ToArray();
            Assert.IsFalse(properties[0].IsVirtual);
            Assert.IsFalse(properties[0].IsAbstract);
            Assert.IsTrue(properties[0].IsOverride);
            Assert.IsFalse(properties[0].IsSealed);
            Assert.IsFalse(properties[1].IsVirtual);
            Assert.IsFalse(properties[1].IsAbstract);
            Assert.IsFalse(properties[1].IsOverride);
            Assert.IsFalse(properties[1].IsSealed);

        }

        [TestMethod]
        [TestCategory(OverrideCategory)]
        public void Can_get_override_for_property()
        {
            var csharpCode = @"
public class Foo1
{
  public override string Bar1{get;}
  public string Bar2{get;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var properties = root.Classes.First().Properties.ToArray();
            Assert.IsFalse(properties[0].IsVirtual);
            Assert.IsFalse(properties[0].IsAbstract);
            Assert.IsTrue(properties[0].IsOverride);
            Assert.IsFalse(properties[0].IsSealed);
            Assert.IsFalse(properties[1].IsVirtual);
            Assert.IsFalse(properties[1].IsAbstract);
            Assert.IsFalse(properties[1].IsOverride);
            Assert.IsFalse(properties[1].IsSealed);

        }

        #endregion

        #region abstract tests
        [TestMethod]
        [TestCategory(AbstractCategory)]
        public void Can_get_abstract_for_method()
        {
            var csharpCode = @"
public class Foo1
{
  public abstract string Bar1(){}
  public string Bar2(){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Methods.ToArray();
            Assert.IsFalse(methods[0].IsVirtual);
            Assert.IsTrue(methods[0].IsAbstract);
            Assert.IsFalse(methods[0].IsOverride);
            Assert.IsFalse(methods[0].IsSealed);
            Assert.IsFalse(methods[1].IsVirtual);
            Assert.IsFalse(methods[1].IsAbstract);
            Assert.IsFalse(methods[1].IsOverride);
            Assert.IsFalse(methods[1].IsSealed);

        }

        [TestMethod]
        [TestCategory(AbstractCategory)]
        public void Can_get_abstract_for_property()
        {
            var csharpCode = @"
public class Foo1
{
  public abstract string Bar1{get;}
  public string Bar2{get;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var properties = root.Classes.First().Properties.ToArray();
            Assert.IsFalse(properties[0].IsVirtual);
            Assert.IsTrue(properties[0].IsAbstract);
            Assert.IsFalse(properties[0].IsOverride);
            Assert.IsFalse(properties[0].IsSealed);
            Assert.IsFalse(properties[1].IsVirtual);
            Assert.IsFalse(properties[1].IsAbstract);
            Assert.IsFalse(properties[1].IsOverride);
            Assert.IsFalse(properties[1].IsSealed);

        }

        [TestMethod]
        [TestCategory(AbstractCategory)]
        public void Can_get_abstract_for_class()
        {
            var csharpCode = @"
public abstract class Foo1{}
public class Foo2{}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var classes = root.Classes.ToArray();
            Assert.IsTrue(classes[0].IsAbstract);
            Assert.IsFalse(classes[0].IsSealed);
            Assert.IsFalse(classes[1].IsAbstract);
            Assert.IsFalse(classes[1].IsSealed);

        }

        #endregion

        #region sealed tests
        [TestMethod]
        [TestCategory(SealedCategory)]
        public void Can_get_sealed_for_method()
        {
            var csharpCode = @"
public class Foo1
{
  public sealed string Bar1(){}
  public string Bar2(){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Methods.ToArray();
            Assert.IsFalse(methods[0].IsVirtual);
            Assert.IsFalse(methods[0].IsAbstract);
            Assert.IsFalse(methods[0].IsOverride);
            Assert.IsTrue(methods[0].IsSealed);
            Assert.IsFalse(methods[1].IsVirtual);
            Assert.IsFalse(methods[1].IsAbstract);
            Assert.IsFalse(methods[1].IsOverride);
            Assert.IsFalse(methods[1].IsSealed);

        }

        [TestMethod]
        [TestCategory(SealedCategory)]
        public void Can_get_sealed_for_property()
        {
            var csharpCode = @"
public class Foo1
{
  public sealed string Bar1{get;}
  public string Bar2{get;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var properties = root.Classes.First().Properties.ToArray();
            Assert.IsFalse(properties[0].IsVirtual);
            Assert.IsFalse(properties[0].IsAbstract);
            Assert.IsFalse(properties[0].IsOverride);
            Assert.IsTrue(properties[0].IsSealed);
            Assert.IsFalse(properties[1].IsVirtual);
            Assert.IsFalse(properties[1].IsAbstract);
            Assert.IsFalse(properties[1].IsOverride);
            Assert.IsFalse(properties[1].IsSealed);

        }

        [TestMethod]
        [TestCategory(SealedCategory)]
        public void Can_get_sealed_for_class()
        {
            var csharpCode = @"
public sealed class Foo1{}
public class Foo2{}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var classes = root.Classes.ToArray();
            Assert.IsFalse(classes[0].IsAbstract);
            Assert.IsTrue(classes[0].IsSealed);
            Assert.IsFalse(classes[1].IsAbstract);
            Assert.IsFalse(classes[1].IsSealed);

        }

        #endregion

        #region new tests
        [TestMethod]
        [TestCategory(NewCategory)]
        public void Can_get_new_for_method()
        {
            var csharpCode = @"
public class Foo1
{
  public new string Bar1(){}
  public string Bar2(){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Methods.ToArray();
            Assert.IsFalse(methods[0].IsVirtual);
            Assert.IsFalse(methods[0].IsAbstract);
            Assert.IsFalse(methods[0].IsOverride);
            Assert.IsFalse(methods[0].IsSealed);
            Assert.IsFalse(methods[1].IsVirtual);
            Assert.IsFalse(methods[1].IsAbstract);
            Assert.IsFalse(methods[1].IsOverride);
            Assert.IsFalse(methods[1].IsSealed);

        }

        [TestMethod]
        [TestCategory(NewCategory)]
        public void Can_get_new_for_property()
        {
            var csharpCode = @"
public class Foo1
{
  public new string Bar1{get;}
  public string Bar2{get;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Properties.ToArray();
            Assert.IsFalse(methods[0].IsVirtual);
            Assert.IsFalse(methods[0].IsAbstract);
            Assert.IsFalse(methods[0].IsOverride);
            Assert.IsFalse(methods[0].IsSealed);
            Assert.IsFalse(methods[1].IsVirtual);
            Assert.IsFalse(methods[1].IsAbstract);
            Assert.IsFalse(methods[1].IsOverride);
            Assert.IsFalse(methods[1].IsSealed);

        }

        #endregion

        #region static tests
        [TestMethod]
        [TestCategory(StaticCategory)]
        public void Can_get_static_for_field()
        {
            var csharpCode = @"
public class Foo1
{
  public static string Bar1;
  public string Bar2
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var fields = root.Classes.First().Fields.ToArray();
            Assert.IsTrue(fields[0].IsStatic);
            Assert.IsFalse(fields[1].IsStatic);

        }

        [TestMethod]
        [TestCategory(StaticCategory)]
        public void Can_get_static_for_method()
        {
            var csharpCode = @"
public class Foo1
{
  public static abstract virtual override sealed string Bar1(){}
  public string Bar2(){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Methods.ToArray();
            Assert.IsTrue(methods[0].IsVirtual);
            Assert.IsTrue(methods[0].IsAbstract);
            Assert.IsTrue(methods[0].IsOverride);
            Assert.IsTrue(methods[0].IsSealed);
            Assert.IsTrue(methods[0].IsStatic);
            Assert.IsFalse(methods[1].IsVirtual);
            Assert.IsFalse(methods[1].IsAbstract);
            Assert.IsFalse(methods[1].IsOverride);
            Assert.IsFalse(methods[1].IsSealed);
            Assert.IsFalse(methods[1].IsStatic);

        }

        [TestMethod]
        [TestCategory(StaticCategory)]
        public void Can_get_static_for_property()
        {
            var csharpCode = @"
public class Foo1
{
  public static string Bar1{get;}
  public string Bar2{get;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var properties = root.Classes.First().Properties.ToArray();
            Assert.IsFalse(properties[0].IsVirtual);
            Assert.IsFalse(properties[0].IsAbstract);
            Assert.IsFalse(properties[0].IsOverride);
            Assert.IsFalse(properties[0].IsSealed);
            Assert.IsTrue(properties[0].IsStatic);
            Assert.IsFalse(properties[1].IsVirtual);
            Assert.IsFalse(properties[1].IsAbstract);
            Assert.IsFalse(properties[1].IsOverride);
            Assert.IsFalse(properties[1].IsSealed);
            Assert.IsFalse(properties[1].IsStatic);


        }

        [TestMethod]
        [TestCategory(StaticCategory)]
        public void Can_get_static_for_class()
        {
            var csharpCode = @"
public abstract static class Foo1{}
public class Foo2{}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var classes = root.Classes.ToArray();
            Assert.IsTrue(classes[0].IsAbstract);
            Assert.IsFalse(classes[0].IsSealed);
            Assert.IsTrue(classes[0].IsStatic);
            Assert.IsFalse(classes[1].IsAbstract);
            Assert.IsFalse(classes[1].IsSealed);
            Assert.IsFalse(classes[1].IsStatic);

        }

        #endregion

        #region enum tests
        [TestMethod]
        [TestCategory(StaticCategory)]
        public void Can_get_underlyig_type_for_enum()
        {
            var csharpCode = @"
public enum Foo1 : byte {}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var en = root.Enums.First();
            Assert.AreEqual("Byte", en.UnderlyingType.Name);
            Assert.AreEqual("System.Byte", en.UnderlyingType.QualifiedName);

        }
        #endregion

        #region implemented interfaces tests

        private string csharpCodeForInterfaceTests = @"
public class Foo1A : IFooA, IFooC { }
public class Foo1B : Foo1A, IFooB, IFooC { }
public class Foo1C : Foo1B {} 

public struct Foo2A : IFooA, IFooC { }

public interface IFoo3A : IFooA, IFooC { }
public interface IFoo3B : IFoo3A, IFooB, IFooC { }
public interface IFoo3C : IFoo3B {}

public interface IFooA{}
public interface IFooB{}
public interface IFooC{}

";


        [TestMethod]
        [TestCategory(ImplementedInterfacesCategory)]
        public void Can_get_implemented_interfaces_for_class()
        {
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeForInterfaceTests);
            var classes = root.Classes.ToArray();
            Assert.AreEqual(2, classes[0].ImplementedInterfaces.Count());
            Assert.AreEqual(2, classes[1].ImplementedInterfaces.Count());
            Assert.AreEqual(0, classes[2].ImplementedInterfaces.Count());
        }

        [TestMethod]
        [TestCategory(ImplementedInterfacesCategory)]
        public void Can_get_all_implemented_interfaces_for_class()
        {
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeForInterfaceTests);
            var classes = root.Classes.ToArray();
            Assert.AreEqual(2, classes[0].AllImplementedInterfaces.Count());
            Assert.AreEqual(3, classes[1].AllImplementedInterfaces.Count());
            Assert.AreEqual(3, classes[2].AllImplementedInterfaces.Count());
        }


        [TestMethod]
        [TestCategory(ImplementedInterfacesCategory)]
        public void Can_get_implemented_interfaces_for_structure()
        {
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeForInterfaceTests);
            var structures = root.Structures.ToArray();
            Assert.AreEqual(2, structures[0].ImplementedInterfaces.Count());
        }

        [TestMethod]
        [TestCategory(ImplementedInterfacesCategory)]
        public void Can_get_all_implemented_interfaces_for_structure()
        {
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeForInterfaceTests);
            var structures = root.Structures.ToArray();
            Assert.AreEqual(2, structures[0].AllImplementedInterfaces.Count());
        }


        [TestMethod]
        [TestCategory(ImplementedInterfacesCategory)]
        public void Can_get_implemented_interfaces_for_interface()
        {
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeForInterfaceTests);
            var interfaces = root.Interfaces.ToArray();
            Assert.AreEqual(2, interfaces[0].ImplementedInterfaces.Count());
            Assert.AreEqual(3, interfaces[1].ImplementedInterfaces.Count());
            Assert.AreEqual(1, interfaces[2].ImplementedInterfaces.Count());
        }

        [TestMethod]
        [TestCategory(ImplementedInterfacesCategory)]
        public void Can_get_all_implemented_interfaces_for_interface()
        {
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeForInterfaceTests);
            var interfaces = root.Interfaces.ToArray();
            Assert.AreEqual(2, interfaces[0].AllImplementedInterfaces.Count());
            Assert.AreEqual(4, interfaces[1].AllImplementedInterfaces.Count());
            Assert.AreEqual(5, interfaces[2].AllImplementedInterfaces.Count());
        }

        #endregion

        #region base type tests
        [TestMethod, TestCategory(BaseTypeCategory)]
        public void Can_get_base_type_for_class()
        {
            var csharpCode = @"
public class Foo1  {}
public class Foo2 : Foo1 {}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var classes = root.Classes.ToArray();
            Assert.IsNotNull(classes[0].BaseType);
            Assert.AreEqual("System.Object", classes[0].BaseType.QualifiedName);
            Assert.IsNotNull(classes[1].BaseType);
            Assert.AreEqual("Foo1", classes[1].BaseType.QualifiedName);

        }
        #endregion

        #region parameter tests
        [TestMethod]
        [TestCategory(ParameterAndMethodCategory)]
        public void Can_get_parameters_for_methods_in_class()
        {
            var csharpCode = @"
public class Foo  
{
   public string Foo1(){}
   public string Foo2(int i){}
   public string Foo3(int i, string s){}
   public string Foo4(ref int i, out string s, params string[] moreStrings){}
   public string Foo5(this A a, int i = 0, string s = ""){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Methods.ToArray();
            Assert.AreEqual(0, methods[0].Parameters.Count());
            Assert.AreEqual(1, methods[1].Parameters.Count());
            Assert.AreEqual(2, methods[2].Parameters.Count());
            Assert.AreEqual(3, methods[3].Parameters.Count());
            ParameterCheck(methods[1].Parameters.First(), 0, "i", "Int32");
            ParameterCheck(methods[2].Parameters.First(), 0, "i", "Int32");
            ParameterCheck(methods[2].Parameters.Last(), 1, "s", "String");
            var parameters = methods[3].Parameters.ToArray();
            ParameterCheck(parameters[0], 0, "i", "Int32", isRef: true);
            ParameterCheck(parameters[1], 1, "s", "String", isOut: true);
            ParameterCheck(parameters[2], 2, "moreStrings", "String[]", isParamArray: true);
            parameters = methods[4].Parameters.ToArray();
            ParameterCheck(parameters[0], 0, "a", "A");
            ParameterCheck(parameters[1], 1, "i", "Int32", isOptional: true);
            ParameterCheck(parameters[2], 2, "s", "String", isOptional: true);

        }

        [TestMethod]
        [TestCategory(ParameterAndMethodCategory)]
        public void Can_get_param_array_type_name()
        {
            var csharpCode = @"
public class Foo  
{
   public string Foo4(params string[] moreStrings){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var parameter = root.Classes.First().Methods.First().Parameters.First();
            ParameterCheck(parameter, 0, "moreStrings", "String[]", isParamArray: true);

        }

        [TestMethod]
        [TestCategory(ParameterAndMethodCategory)]
        public void Can_get_parameters_for_extension_methods()
        {
            var csharpCode = @"
public static class Foo  
{
   public static string Foo5(this A a, int i = 0, string s = ""){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var parameters = root.Classes.First().Methods.First().Parameters.ToArray();
            ParameterCheck(parameters[0], 0, "a", "A");
            ParameterCheck(parameters[1], 1, "i", "Int32", isOptional: true);
            ParameterCheck(parameters[2], 2, "s", "String", isOptional: true);

        }

        [TestMethod]
        [TestCategory(ParameterAndMethodCategory)]
        public void Can_determine_extension_method()
        {
            var csharpCode = @"
public static class Foo  
{
   public static string Foo5(this A a, int i = 0, string s = ""){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var method = root.Classes.First().Methods.First();
            Assert.IsTrue(method.IsExtensionMethod);

        }


        private void ParameterCheck(IParameter parm, int ordinal, string name, string typeName,
                bool isOut = false, bool isRef = false, bool isParamArray = false,
                bool isOptional = false)
        {
            Assert.AreEqual(name, parm.Name);
            Assert.AreEqual(typeName, parm.Type.Name);
            Assert.AreEqual(isOut, parm.IsOut);
            Assert.AreEqual(isRef, parm.IsRef);
            Assert.AreEqual(isParamArray, parm.IsParamArray);
            Assert.AreEqual(isOptional, parm.IsOptional);
            Assert.AreEqual(ordinal, parm.Ordinal);
        }
        #endregion

        #region returned type tests
        [TestMethod]
        [TestCategory(ReturnTypeNameCategory)]
        public void Can_get_return_type_name_for_method()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1(){}
   public Int32 Foo2(int i){}
   public System.Diagnostics.Tracing.EventKeyword Foo3(int i, string s){}
   public BadName Foo4(int i){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Methods.ToArray();
            Assert.AreEqual("System.String", methods[0].RequestValue("TypeName"));
            Assert.AreEqual("System.Int32", methods[1].RequestValue("TypeName"));
            Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", methods[2].RequestValue("TypeName"));
            Assert.AreEqual("BadName", methods[3].RequestValue("TypeName"));
        }

        [TestMethod]
        [TestCategory(ReturnTypeNameCategory)]
        public void Can_get_return_type_name_for_property()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
   public Int32 Foo2{get; set;}
   public System.Diagnostics.Tracing.EventKeyword Foo3{}
   public BadName Foo4{ get; set; }
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var properties = root.Classes.First().Properties.ToArray();
            Assert.AreEqual("System.String", properties[0].RequestValue("TypeName"));
            Assert.AreEqual("System.Int32", properties[1].RequestValue("TypeName"));
            Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", properties[2].RequestValue("TypeName"));
            Assert.AreEqual("BadName", properties[3].RequestValue("TypeName"));
            Assert.AreEqual("Foo4", properties[3].RequestValue("Name"));
        }

        [TestMethod]
        [TestCategory(ReturnTypeNameCategory)]
        public void Can_get_return_type_name_for_field()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1;
   public Int32 Foo2;
   public System.Diagnostics.Tracing.EventKeyword Foo3;
   public BadName Foo4;
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var methods = root.Classes.First().Fields.ToArray();
            Assert.AreEqual("System.String", methods[0].RequestValue("TypeName"));
            Assert.AreEqual("System.Int32", methods[1].RequestValue("TypeName"));
            Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", methods[2].RequestValue("TypeName"));
            Assert.AreEqual("BadName", methods[3].RequestValue("TypeName"));
        }

        [TestMethod]
        [TestCategory(ReturnTypeNameCategory)]
        public void Can_get_return_type_name_for_parameter()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public void Foo3(string s, Int32 i, System.Diagnostics.Tracing.EventKeyword keyword, BadName whatever){}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var parameters = root.Classes.First().Methods.First().Parameters.ToArray();
            Assert.AreEqual("System.String", parameters[0].RequestValue("TypeName"));
            Assert.AreEqual("System.Int32", parameters[1].RequestValue("TypeName"));
            Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", parameters[2].RequestValue("TypeName"));
            Assert.AreEqual("BadName", parameters[3].RequestValue("TypeName"));
        }
        #endregion

        #region namespace tests
        [TestMethod]
        [TestCategory(NamespaceCategory)]
        public void Can_get_all_namespaces_from_root()
        {
            var csharpCode = @"
namespace Foo
{
   namespace Bar1
   {
       namespace FooBar
       { public enum Foo1 : byte {} }
   }
   namespace Bar2 {}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var namespaces = root.ChildNamespaces;
            var allNamespaces = root.DescendantNamespaces;
            var nonEmptyNamespaces = root.Namespaces;
            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual(4, allNamespaces.Count());
            Assert.AreEqual(3, allNamespaces.First().DescendantNamespaces.Count());
            Assert.AreEqual(1, nonEmptyNamespaces.Count());
            Assert.AreEqual("FooBar", nonEmptyNamespaces.First().Name);
            Assert.AreEqual("Foo.Bar1.FooBar", nonEmptyNamespaces.First().QualifiedName);
        }

        [TestMethod]
        [TestCategory(NamespaceCategory)]
        public void Does_not_crash_on_empty_namespaces_from_root()
        {
            var csharpCode = @"
namespace Foo
{
   namespace Bar1
   {
       namespace FooBar
       {  }
   }
   namespace Bar2 {}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var namespaces = root.ChildNamespaces;
            var allNamespaces = root.DescendantNamespaces;
            var nonEmptyNamespaces = root.Namespaces;
            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual(4, allNamespaces.Count());
            Assert.AreEqual(3, allNamespaces.First().DescendantNamespaces.Count());
            Assert.AreEqual(0, nonEmptyNamespaces.Count());
        }

        [TestMethod]
        [TestCategory(NamespaceCategory)]
        public void Does_not_crash_on_no_namespaces_from_root()
        {
            var csharpCode = @"
public enum Foo1 : byte {}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var namespaces = root.ChildNamespaces;
            var allNamespaces = root.DescendantNamespaces;
            var nonEmptyNamespaces = root.Namespaces;
            Assert.AreEqual(0, namespaces.Count());
            Assert.AreEqual(0, allNamespaces.Count());
            Assert.AreEqual(0, nonEmptyNamespaces.Count());
        }

        [TestMethod]
        [TestCategory(NamespaceCategory)]
        public void Can_get_all_namespaces_from_namespace()
        {
            var csharpCode = @"
namespace Foo
{
   namespace Bar1
   {
       namespace FooBar
       { public enum Foo1 : byte {} }
   }
   namespace Bar2 {}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var topNamespace = root.ChildNamespaces.First();
            var namespaces = topNamespace.Namespaces;
            var allNamespaces = topNamespace.DescendantNamespaces;
            var childNamespaces = topNamespace.ChildNamespaces;
            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual(3, allNamespaces.Count());
            Assert.AreEqual(1, allNamespaces.First().Namespaces.Count());
            Assert.AreEqual(2, childNamespaces.Count());
            Assert.AreEqual("FooBar", namespaces.First().Name);
            Assert.AreEqual("Foo.Bar1.FooBar", namespaces.First().QualifiedName);
        }

        [TestMethod]
        [TestCategory(NamespaceCategory)]
        public void Does_not_crash_on_empty_namespaces_from_namespace()
        {
            var csharpCode = @"
namespace Foo
{
   namespace Bar1
   {
       namespace FooBar
       {  }
   }
   namespace Bar2 {}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var topNamespace = root.DescendantNamespaces.First();
            var namespaces = topNamespace.ChildNamespaces;
            var allNamespaces = topNamespace.DescendantNamespaces;
            var nonEmptyNamespaces = topNamespace.Namespaces;
            Assert.AreEqual(2, namespaces.Count());
            Assert.AreEqual(3, allNamespaces.Count());
            Assert.AreEqual(1, allNamespaces.First().ChildNamespaces.Count());
            Assert.AreEqual(0, nonEmptyNamespaces.Count());
        }

        [TestMethod]
        [TestCategory(NamespaceCategory)]
        public void Does_not_crash_on_no_namespaces_from_namespace()
        {
            var csharpCode = @"
namespace Namespace1
{
   public enum Foo1 : byte {}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var topNamespace = root.Namespaces.First();
            var namespaces = topNamespace.Namespaces;
            var allNamespaces = topNamespace.Namespaces;
            var nonEmptyNamespaces = topNamespace.DescendantNamespaces;
            Assert.AreEqual(0, namespaces.Count());
            Assert.AreEqual(0, allNamespaces.Count());
            Assert.AreEqual(0, nonEmptyNamespaces.Count());
        }
        #endregion

        #region member type
        [TestMethod]
        [TestCategory(MemberKindCategory)]
        public void Can_get_member_type_for_members()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
   public Int32 Foo2(){}
   public string Foo3;
   public class Foo4{}
   public struct Foo5{}
   public interface Foo6{}
   public enum Foo7{}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var members = root.Classes.First().Members.ToArray();
            Assert.AreEqual(MemberKind.Property, members[0].MemberKind);
            Assert.AreEqual(MemberKind.Method,   members[1].MemberKind);
            Assert.AreEqual(MemberKind.Field,    members[2].MemberKind);
            Assert.AreEqual(MemberKind.Class,    members[3].MemberKind);
            Assert.AreEqual(MemberKind.Structure,members[4].MemberKind);
            Assert.AreEqual(MemberKind.Interface,members[5].MemberKind);
            Assert.AreEqual(MemberKind.Enum,     members[6].MemberKind);
        }

        [TestMethod]
        [TestCategory(MemberKindCategory)]
        public void Can_get_member_type_for_members_via_requestValue()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
   public Int32 Foo2(){}
   public string Foo3;
   public class Foo4{}
   public struct Foo5{}
   public interface Foo6{}
   public enum Foo7{}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var members = root.Classes.First().Members.ToArray();
            Assert.AreEqual(MemberKind.Property, members[0].RequestValue("MemberKind"));
            Assert.AreEqual(MemberKind.Method, members[1].RequestValue("MemberKind"));
            Assert.AreEqual(MemberKind.Field, members[2].RequestValue("MemberKind"));
            Assert.AreEqual(MemberKind.Class, members[3].RequestValue("MemberKind"));
            Assert.AreEqual(MemberKind.Structure, members[4].RequestValue("MemberKind"));
            Assert.AreEqual(MemberKind.Interface, members[5].RequestValue("MemberKind"));
            Assert.AreEqual(MemberKind.Enum, members[6].RequestValue("MemberKind"));
        }

        [TestMethod]
        [TestCategory(MemberKindCategory)]
        public void RequestValue_returns_null_if_property_not_found()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var members = root.Classes.First().Members.ToArray();
            Assert.IsNull(members[0].RequestValue("MemberKindX"));
        }


        [TestMethod]
        [TestCategory(MemberKindCategory)]
        public void Can_get_value_from_parameter_via_RequestValue()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1(int Bar) {}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var parameters = root.Classes.First().Methods.First().Parameters.ToArray();
            Assert.AreEqual(false, (bool)parameters[0].RequestValue("IsOut"));
        }
        #endregion

        #region property access
        [TestMethod]
        [TestCategory(PropertyAccessCategory )]
        public void Can_get_property_access()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
   public string Foo2{get;}
   public string Foo3{set;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var properties = root.Classes.First().Properties .ToArray();
            Assert.IsTrue(properties[0].CanGet);
            Assert.IsTrue(properties[0].CanSet);
            Assert.IsTrue(properties[1].CanGet);
            Assert.IsFalse(properties[1].CanSet);
            Assert.IsFalse(properties[2].CanGet);
            Assert.IsTrue(properties[2].CanSet);
        }


        #endregion

        #region miscellaneous
        [TestMethod]
        [TestCategory(MiscellaneousCategory )]
        public void Can_get_class_name_for_class()
        {
            var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var cl =(RDomClass) root.Classes.First();
            Assert.AreEqual("Foo",cl.ClassName) ;
        }

        [TestMethod]
        [TestCategory(MiscellaneousCategory)]
        public void Can_get_root_classes()
        {
            var csharpCode = @"
using System
public class Foo  {}
namespace Namespace1
{
   public class Foo1{}
   public class Foo2{}
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.ToArray();
            Assert.AreEqual("Foo", ((RDomClass)cl[0]).ClassName);
            Assert.AreEqual("Foo1", ((RDomClass)cl[1]).ClassName);
            Assert.AreEqual("Foo2", ((RDomClass)cl[2]).ClassName);
        }

  
        #endregion
    }
}
