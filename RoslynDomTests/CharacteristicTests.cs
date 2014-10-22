using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.StatePrinter;
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
      private const string IsNestedCategory = "IsNested";
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
      [TestMethod, TestCategory(ReturnedTypeCategory)]
      public void Can_get_field_return_type()
      {
         var csharpCode = @"
            public class Foo
            {
            public string Bar;
            }";
         VerifyType(csharpCode, r => r.Classes.First().Fields.First(), "System.String");
      }

      [TestMethod, TestCategory(ReturnedTypeCategory)]
      public void Can_get_property_return_type()
      {
         var csharpCode = @"
            public class Foo
            {
                public int Bar{get;}
            }";
         VerifyType(csharpCode, r => r.Classes.First().Properties.First(), "System.Int32");
      }

      [TestMethod, TestCategory(ReturnedTypeCategory)]
      public void Can_get_method_return_type()
      {
         var csharpCode = @"
            public class Foo
            {
                public Namespace1.A  Bar() {}
            }";
         VerifyType(csharpCode, r => r.Classes.First().Methods.First(), "Namespace1.A");
      }

      [TestMethod, TestCategory(ReturnedTypeCategory)]
      public void Can_get_method_predefined_return_type()
      {
         var csharpCode = @"
using System;
public class Foo
{
   public string  Bar() {}
   public String  Bar() {}
}
";
         var item1 = VerifyType(csharpCode, r => r.Classes.First().Methods.ElementAt(0), "System.String");
         var item2 = VerifyType(csharpCode, r => r.Classes.First().Methods.ElementAt(1), "System.String");
         item1.ReturnType.DisplayAlias = true;
         item2.ReturnType.DisplayAlias = false;
      }

      [TestMethod, TestCategory(ReturnedTypeCategory)]
      public void Can_get_event_return_type()
      {
         var csharpCode = @"
            public class Foo
            {
                public event Func<Namespace1.A>  Bar;
            }";
         VerifyEvent(csharpCode, r => r.Classes.First().Events.First(), "Func<Namespace1.A>");
      }

      [TestMethod, TestCategory(ReturnedTypeCategory)]
      public void Can_get_event_predefined_return_type()
      {
         var csharpCode = @"
using System;
public class Foo
{
   public event EventHandler  Bar;
   event EventHandler  Bar;
}
";
         var item1 = VerifyEvent(csharpCode, r => r.Classes.First().Events.ElementAt(0), "System.EventHandler");
         item1.Type.DisplayAlias = true;
      }


      #endregion

      #region access modifier tests
      [TestMethod, TestCategory(AccessModifierCategory)]
      public void Can_get_access_modifier_for_class()
      {
         var csharpCode = @"
                public class Foo1{}
                private class Foo2{}
                protected class Foo3{}
                protected internal class Foo4{}
                internal class Foo5{}
                class Foo6{}";
         VerifyAccessModifier(csharpCode, r => r.Classes.ToArray(),
                          AccessModifier.Public,
                          AccessModifier.Private,
                          AccessModifier.Protected,
                          AccessModifier.ProtectedOrInternal,
                          AccessModifier.Internal,
                          AccessModifier.Internal);

      }

      [TestMethod, TestCategory(AccessModifierCategory)]
      public void Can_get_access_modifier_for_Interface()
      {
         var csharpCode = @"
                public interface Foo1{}
                private interface Foo2{}
                protected interface Foo3{}
                protected internal interface Foo4{}
                internal interface Foo5{}
                interface Foo6{}";
         VerifyAccessModifier(csharpCode, r => r.Interfaces.ToArray(),
                              AccessModifier.Public,
                              AccessModifier.Private,
                              AccessModifier.Protected,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.Internal,
                              AccessModifier.Internal);
      }

      [TestMethod, TestCategory(AccessModifierCategory)]
      public void Can_get_access_modifier_for_Structure()
      {
         var csharpCode = @"
                public struct Foo1{}
                private struct Foo2{}
                protected struct Foo3{}
                protected internal struct Foo4{}
                internal struct Foo5{}
                struct Foo6{}";
         VerifyAccessModifier(csharpCode, r => r.Structures.ToArray(),
                              AccessModifier.Public,
                              AccessModifier.Private,
                              AccessModifier.Protected,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.Internal,
                              AccessModifier.Internal);
      }

      [TestMethod, TestCategory(AccessModifierCategory)]
      public void Can_get_access_modifier_for_enum()
      {
         var csharpCode = @"
                public enum Foo1{}
                private enum Foo2{}
                protected enum Foo3{}
                protected internal enum Foo4{}
                internal enum Foo5{}
                enum Foo6{}";
         VerifyAccessModifier(csharpCode, r => r.Enums.ToArray(),
                              AccessModifier.Public,
                              AccessModifier.Private,
                              AccessModifier.Protected,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.Internal,
                              AccessModifier.Internal);
      }

      [TestMethod, TestCategory(AccessModifierCategory)]
      public void Can_get_access_modifier_for_members_in_class()
      {
         var csharpCode = @"
                public class Foo1
                {
                    public string Bar1;
                    public string Bar2 {get;}
                    public string Bar3(){}
                    private string Bar4;
                    private string Bar5 {get;}
                    private string Bar6(){}
                    protected string Bar7;
                    protected string Bar8 {get;}
                    protected string Bar9(){}
                    protected internal string Bar10;
                    protected internal string Bar11 {get;}
                    protected internal string Bar12(){}
                    internal string Bar13;
                    internal string Bar14 {get;}
                    internal string Bar15(){}
                    string Bar16;
                    string Bar17 {get;}
                    string Bar18(){}
                }";
         VerifyAccessModifier(csharpCode, r => r.Classes.First().Members.ToArray(),
                              AccessModifier.Public,
                              AccessModifier.Public,
                              AccessModifier.Public,
                              AccessModifier.Private,
                              AccessModifier.Private,
                              AccessModifier.Private,
                              AccessModifier.Protected,
                              AccessModifier.Protected,
                              AccessModifier.Protected,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.Internal,
                              AccessModifier.Internal,
                              AccessModifier.Internal,
                              AccessModifier.Private,
                              AccessModifier.Private,
                              AccessModifier.Private);
      }

      [TestMethod, TestCategory(AccessModifierCategory)]
      public void Can_get_access_modifier_for_members_in_structure()
      {
         var csharpCode = @"
                public struct Foo1
                {
                    public string Bar1;
                    public string Bar2 {get;}
                    public string Bar3(){}
                    public static Complex operator +(Complex c1, Complex c2){}
                    public static explicit operator Digit(byte b) {}
                    private string Bar4;
                    private string Bar5 {get;}
                    private string Bar6(){}
                    protected string Bar7;
                    protected string Bar8 {get;}
                    protected string Bar9(){}
                    protected internal string Bar10;
                    protected internal string Bar11 {get;}
                    protected internal string Bar12(){}
                    internal string Bar13;
                    internal string Bar14 {get;}
                    internal string Bar15(){}
                    string Bar16;
                    string Bar17 {get;}
                    string Bar18(){}
             }";
         VerifyAccessModifier(csharpCode, r => r.Structures.First().Members.ToArray(),
                              AccessModifier.Public,
                              AccessModifier.Public,
                              AccessModifier.Public,
                              AccessModifier.Public,
                              AccessModifier.Public,
                              AccessModifier.Private,
                              AccessModifier.Private,
                              AccessModifier.Private,
                              AccessModifier.Protected,
                              AccessModifier.Protected,
                              AccessModifier.Protected,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.ProtectedOrInternal,
                              AccessModifier.Internal,
                              AccessModifier.Internal,
                              AccessModifier.Internal,
                              AccessModifier.Private,
                              AccessModifier.Private,
                              AccessModifier.Private);
      }

      [TestMethod, TestCategory(AccessModifierCategory)]
      public void Can_get_access_modifier_for_members_in_interface()
      {
         var csharpCode = @"
            public interface Foo1
            {
                string Bar16;
                string Bar17 {get;}
                string Bar18(){}
            }";
         VerifyAccessModifier(csharpCode, r => r.Interfaces.First().Members.ToArray(),
                              AccessModifier.None,
                              AccessModifier.None,
                              AccessModifier.None);
      }
      #endregion

      #region virtual tests
      [TestMethod, TestCategory(VirtualCategory)]
      public void Can_get_virtual_for_method()
      {
         var csharpCode = @"
            public class Foo1
            {
              public virtual string Bar1(){}
              public string Bar2(){}
            }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Methods.ToArray(),
                     Tuple.Create(true, false, false, false, false, false),
                     Tuple.Create(false, false, false, false, false, false));
      }


      [TestMethod, TestCategory(VirtualCategory)]
      public void Can_get_virtual_for_property()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public virtual string Bar1{get;}
                  public string Bar2{get;}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Properties.ToArray(),
                    Tuple.Create(true, false, false, false, false, false),
                    Tuple.Create(false, false, false, false, false, false));

      }

      #endregion

      #region override tests
      [TestMethod, TestCategory(OverrideCategory)]
      public void Can_get_override_for_method()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public override string Bar1(){}
                  public string Bar2(){}
                }                ";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Methods.ToArray(),
                    Tuple.Create(false, false, true, false, false, false),
                    Tuple.Create(false, false, false, false, false, false));
      }

      [TestMethod, TestCategory(OverrideCategory)]
      public void Can_get_override_for_property()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public override string Bar1{get;}
                  public string Bar2{get;}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Properties.ToArray(),
                    Tuple.Create(false, false, true, false, false, false),
                    Tuple.Create(false, false, false, false, false, false));

      }

      #endregion

      #region abstract tests
      [TestMethod, TestCategory(AbstractCategory)]
      public void Can_get_abstract_for_method()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public abstract string Bar1(){}
                  public string Bar2(){}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Methods.ToArray(),
                    Tuple.Create(false, true, false, false, false, false),
                    Tuple.Create(false, false, false, false, false, false));
      }

      [TestMethod, TestCategory(AbstractCategory)]
      public void Can_get_abstract_for_property()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public abstract string Bar1{get;}
                  public string Bar2{get;}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Properties.ToArray(),
                    Tuple.Create(false, true, false, false, false, false),
                    Tuple.Create(false, false, false, false, false, false));
      }

      [TestMethod, TestCategory(AbstractCategory)]
      public void Can_get_abstract_for_class()
      {
         var csharpCode = @"
                public abstract class Foo1{}
                public class Foo2{}";
         VerifyOOClass(csharpCode, r => r.Classes,
                   Tuple.Create(true, false, false),
                   Tuple.Create(false, false, false));
      }

      #endregion

      #region sealed tests
      [TestMethod, TestCategory(SealedCategory)]
      public void Can_get_sealed_for_method()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public sealed string Bar1(){}
                  public string Bar2(){}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Methods.ToArray(),
                    Tuple.Create(false, false, false, true, false, false),
                    Tuple.Create(false, false, false, false, false, false));

      }

      [TestMethod, TestCategory(SealedCategory)]
      public void Can_get_sealed_for_property()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public sealed string Bar1{get;}
                  public string Bar2{get;}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Properties.ToArray(),
                    Tuple.Create(false, false, false, true, false, false),
                    Tuple.Create(false, false, false, false, false, false));
      }

      [TestMethod, TestCategory(SealedCategory)]
      public void Can_get_sealed_for_class()
      {
         var csharpCode = @"
                public sealed class Foo1{}
                public class Foo2{}";
         VerifyOOClass(csharpCode, r => r.Classes,
                   Tuple.Create(false, true, false),
                   Tuple.Create(false, false, false));
      }

      #endregion

      #region new tests
      [TestMethod, TestCategory(NewCategory)]
      public void Can_get_new_for_method()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public new string Bar1(){}
                  public string Bar2(){}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Methods.ToArray(),
                    Tuple.Create(false, false, false, false, true, false),
                    Tuple.Create(false, false, false, false, false, false));
      }

      [TestMethod, TestCategory(NewCategory)]
      public void Can_get_new_for_property()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public new string Bar1{get;}
                  public string Bar2{get;}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Properties.ToArray(),
                    Tuple.Create(false, false, false, false, true, false),
                    Tuple.Create(false, false, false, false, false, false));

      }

      #endregion

      #region static tests
      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_static_for_field()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public static string Bar1;
                  public string Bar2;
                }";
         VerifyOOField(csharpCode, r => r.Classes.First().Fields.ToArray(),
                   Tuple.Create(true),
                   Tuple.Create(false));
      }

      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_static_for_method()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public static string Bar1(){}
                  public string Bar2(){}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Methods.ToArray(),
                    Tuple.Create(false, false, false, false, false, true),
                    Tuple.Create(false, false, false, false, false, false));
      }

      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_static_for_property()
      {
         var csharpCode = @"
                public class Foo1
                {
                  public static string Bar1{get;}
                  public string Bar2{get;}
                }";
         VerifyOOTypeMember(csharpCode, r => r.Classes.First().Methods.ToArray(),
                   Tuple.Create(false, false, false, false, false, true),
                   Tuple.Create(false, false, false, false, false, false));
      }

      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_static_for_operators()
      {
         var csharpCode = @"
                public struct Foo1
                {
                    public static Complex operator +(Complex c1, Complex c2){}
                    public static Complex operator -(Complex c1, Complex c2){}
                }";
         VerifyStatic(csharpCode, r => r.Structures.First().Operators.ToArray(),
                   Tuple.Create(true),
                   Tuple.Create(true));
      }

      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_static_for_conversion_operators()
      {
         var csharpCode = @"
                public struct Foo1
                {
                    public static explicit operator Digit(byte b) {}
                    public static implicit operator Digit(int b) {}
                }";
         VerifyStatic(csharpCode, r => r.Structures.First().ConversionOperators.ToArray(),
                   Tuple.Create(true),
                   Tuple.Create(true));
      }

      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_static_for_class()
      {
         var csharpCode = @"
                public static class Foo1{}
                public class Foo2{}";
         VerifyOOClass(csharpCode, r => r.Classes,
                   Tuple.Create(false, false, true),
                   Tuple.Create(false, false, false));
      }

      #endregion

      #region is nested and contianing type tests

      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_is_nested_types_in_class()
      {
         var csharpCode = @"
public class Foo1
{
  public class Foo2  {}
  public struct Foo3 {}
  public interface Foo4  {}
  public enum Foo5  {}
}";
         var root = RDom.CSharp.Load(csharpCode);
         var classOuter = root.Classes.First();
         var classNested = classOuter.Classes.First();
         var structureNested = classOuter.Structures.First();
         var interfaceNested = classOuter.Interfaces.First();
         var enumNested = classOuter.Enums.First();
         Assert.IsFalse(classOuter.IsNested);
         Assert.IsTrue(classNested.IsNested);
         Assert.IsTrue(structureNested.IsNested);
         Assert.IsTrue(interfaceNested.IsNested);
         Assert.IsTrue(enumNested.IsNested);
         Assert.AreEqual(classOuter, classNested.ContainingType);
         Assert.AreEqual(classOuter, structureNested.ContainingType);
         Assert.AreEqual(classOuter, interfaceNested.ContainingType);
         Assert.AreEqual(classOuter, enumNested.ContainingType);
         Assert.AreEqual("Foo1", classOuter.OuterName);
         Assert.AreEqual("Foo1+Foo2", classNested.OuterName);
         Assert.AreEqual("Foo1+Foo3", structureNested.OuterName);
         Assert.AreEqual("Foo1+Foo4", interfaceNested.OuterName);
         Assert.AreEqual("Foo1+Foo5", enumNested.OuterName);
      }

      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_is_nested_types_in_structure()
      {
         var csharpCode = @"
public struct Foo1
{
  public class Foo2  {}
  public struct Foo3 {}
  public interface Foo4  {}
  public enum Foo5  {}
}";
         var root = RDom.CSharp.Load(csharpCode);
         var classOuter = root.Structures.First();
         var classNested = classOuter.Classes.First();
         var structureNested = classOuter.Structures.First();
         var interfaceNested = classOuter.Interfaces.First();
         var enumNested = classOuter.Enums.First();
         Assert.IsFalse(classOuter.IsNested);
         Assert.IsTrue(classNested.IsNested);
         Assert.IsTrue(structureNested.IsNested);
         Assert.IsTrue(interfaceNested.IsNested);
         Assert.IsTrue(enumNested.IsNested);
         Assert.AreEqual(classOuter, classNested.ContainingType);
         Assert.AreEqual(classOuter, structureNested.ContainingType);
         Assert.AreEqual(classOuter, interfaceNested.ContainingType);
         Assert.AreEqual(classOuter, enumNested.ContainingType);
         Assert.AreEqual("Foo1", classOuter.OuterName);
         Assert.AreEqual("Foo1+Foo2", classNested.OuterName);
         Assert.AreEqual("Foo1+Foo3", structureNested.OuterName);
         Assert.AreEqual("Foo1+Foo4", interfaceNested.OuterName);
         Assert.AreEqual("Foo1+Foo5", enumNested.OuterName);
      }

      #endregion

      #region enum tests
      [TestMethod, TestCategory(StaticCategory)]
      public void Can_get_underlying_type_for_enum()
      {
         var csharpCode = @"
public enum Foo1 : byte {}
";
         var root = RDom.CSharp.Load(csharpCode);
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
            public interface IFooC{}";


      [TestMethod, TestCategory(ImplementedInterfacesCategory)]
      public void Can_get_implemented_interfaces_for_class()
      {
         var root = RDom.CSharp.Load(csharpCodeForInterfaceTests);
         var classes = root.Classes.ToArray();
         Assert.AreEqual(2, classes[0].ImplementedInterfaces.Count());
         Assert.AreEqual(2, classes[1].ImplementedInterfaces.Count());
         Assert.AreEqual(0, classes[2].ImplementedInterfaces.Count());
      }

      [TestMethod, TestCategory(ImplementedInterfacesCategory)]
      public void Can_get_all_implemented_interfaces_for_class()
      {
         Assert.Inconclusive();
         var root = RDom.CSharp.Load(csharpCodeForInterfaceTests);
         var classes = root.Classes.ToArray();
         Assert.AreEqual(2, classes[0].AllImplementedInterfaces.Count());
         Assert.AreEqual(3, classes[1].AllImplementedInterfaces.Count());
         Assert.AreEqual(3, classes[2].AllImplementedInterfaces.Count());
      }


      [TestMethod, TestCategory(ImplementedInterfacesCategory)]
      public void Can_get_implemented_interfaces_for_structure()
      {
         var root = RDom.CSharp.Load(csharpCodeForInterfaceTests);
         var structures = root.Structures.ToArray();
         Assert.AreEqual(2, structures[0].ImplementedInterfaces.Count());
      }

      [TestMethod, TestCategory(ImplementedInterfacesCategory)]
      public void Can_get_all_implemented_interfaces_for_structure()
      {
         Assert.Inconclusive();
         var root = RDom.CSharp.Load(csharpCodeForInterfaceTests);
         var structures = root.Structures.ToArray();
         Assert.AreEqual(2, structures[0].AllImplementedInterfaces.Count());
      }

      [TestMethod, TestCategory(ImplementedInterfacesCategory)]
      public void Can_get_implemented_interfaces_for_interface()
      {
         var root = RDom.CSharp.Load(csharpCodeForInterfaceTests);
         var interfaces = root.Interfaces.ToArray();
         Assert.AreEqual(2, interfaces[0].ImplementedInterfaces.Count());
         Assert.AreEqual(3, interfaces[1].ImplementedInterfaces.Count());
         Assert.AreEqual(1, interfaces[2].ImplementedInterfaces.Count());
      }

      [TestMethod, TestCategory(ImplementedInterfacesCategory)]
      public void Can_get_all_implemented_interfaces_for_interface()
      {
         Assert.Inconclusive();
         var root = RDom.CSharp.Load(csharpCodeForInterfaceTests);
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
         var root = RDom.CSharp.Load(csharpCode);
         var classes = root.Classes.ToArray();
         Assert.IsNull(classes[0].BaseType);
         Assert.IsNotNull(classes[1].BaseType);
         Assert.AreEqual("Foo1", classes[1].BaseType.QualifiedName);

      }
      #endregion

      #region parameter tests
      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_get_parameters_for_methods_in_class()
      {
         var csharpCode = @"
public class Foo  
{
   public string Foo1(){}
   public string Foo2(int i){}
   public string Foo3(int i, string s){}
   public string Foo4(ref int i, out string s, params string[] moreStrings){}
   public string Foo5(this A a, int i = 0, string s = """"){}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         // StatePrinterApprovals.Verify(root);
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
         ParameterCheck(parameters[2], 2, "moreStrings", "String", isParamArray: true, isArray: true);
         parameters = methods[4].Parameters.ToArray();
         ParameterCheck(parameters[0], 0, "a", "A");
         ParameterCheck(parameters[1], 1, "i", "Int32", isOptional: true, defaultValue: 0);
         ParameterCheck(parameters[2], 2, "s", "String", isOptional: true, defaultValue: "");

         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_get_array_parameter_for_method()
      {
         var csharpCode = @"
public class Foo  
{
   public string Foo2(int[] i){}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var methods = root.Classes.First().Methods.ToArray();
         ParameterCheck(methods[0].Parameters.First(), 0, "i", "Int32", isArray: true);

      }

      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_get_array_parameters_for_method()
      {
         var csharpCode = @"
public class Foo  
{
   public string Foo2(int[] i){}
   public string Foo3(int i, string[] s){}
   public string Foo4(ref int[] i, out string[] s, params string[] moreStrings){}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         // StatePrinterApprovals.Verify(root);
         var methods = root.Classes.First().Methods.ToArray();
         ParameterCheck(methods[0].Parameters.First(), 0, "i", "Int32", isArray: true);
         ParameterCheck(methods[1].Parameters.First(), 0, "i", "Int32");
         ParameterCheck(methods[1].Parameters.Last(), 1, "s", "String", isArray: true);
         var parameters = methods[2].Parameters.ToArray();
         ParameterCheck(parameters[0], 0, "i", "Int32", isRef: true, isArray: true);
         ParameterCheck(parameters[1], 1, "s", "String", isOut: true, isArray: true);
         ParameterCheck(parameters[2], 2, "moreStrings", "String", isArray: true, isParamArray: true);

      }

      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_get_param_array_type_name()
      {
         var csharpCode = @"
public class Foo  
{
   public string Foo4(params string[] moreStrings){}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var parameter = root.Classes.First().Methods.First().Parameters.First();
         ParameterCheck(parameter, 0, "moreStrings", "String", isParamArray: true, isArray: true);

      }

      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_get_parameters_for_extension_methods()
      {
         var csharpCode = @"
public static class Foo  
{
   public static string Foo5(this A a, int i = 0, string s = """"){}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var parameters = root.Classes.First().Methods.First().Parameters.ToArray();
         ParameterCheck(parameters[0], 0, "a", "A");
         ParameterCheck(parameters[1], 1, "i", "Int32", isOptional: true, defaultValue: 0);
         ParameterCheck(parameters[2], 2, "s", "String", isOptional: true, defaultValue: "");

      }

      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_determine_extension_method()
      {
         var csharpCode = @"
public static class Foo  
{
   public static string Foo5(this A a, int i = 0, string s = ""Fred""){}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var method = root.Classes.First().Methods.First();
         Assert.IsTrue(method.IsExtensionMethod);
         Assert.AreEqual(csharpCode, RDom.CSharp.GetSyntaxNode(root).ToFullString());
         var method2 = method.Copy();
         Assert.IsTrue(method.SameIntent(method2));

      }

      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_get_expression_parameter_value()
      {
         var csharpCode = @"
using System;

namespace Test
{
    public class Const
    {
        public const string Test = ""TestContract"";
    }

    [ContractNamespace(Const.Test)]
    class TestClass
    {
        public void Foo(string Foo = Const.Test){}
    }
}";
         var root = RDom.CSharp.Load(csharpCode);
         var parameter = root.RootClasses.ElementAt(1).Methods.First().Parameters.First();
         ParameterCheck(parameter, 0, "Foo", "String", isOptional: true, defaultValue: "TestContract", defaultConstantIdentifier: "Const.Test");
         var actual = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod, TestCategory(ParameterAndMethodCategory)]
      public void Can_get_generic_parameter_value()
      {
         var csharpCode = @"
using System;

namespace Test
{
    class TestClass
    {
        protected override bool SameIntentInternal<TLocal>(TLocal other, bool skipPublicAnnotations){}
    }
}";
         var root = RDom.CSharp.Load(csharpCode);
         var parameters = root.RootClasses.ElementAt(0).Methods.First().Parameters.ToArray();
         ParameterCheck(parameters[0], 0, "other", "TLocal");
         ParameterCheck(parameters[1], 1, "skipPublicAnnotations", "Boolean");
         var actual = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }

      private void ParameterCheck(IParameter parm, int ordinal, string name, string typeName,
                bool isOut = false, bool isRef = false, bool isParamArray = false,
                bool isOptional = false,
                bool isArray = false,
                object defaultValue = null,
                string defaultConstantIdentifier = null)
      {
         Assert.AreEqual(name, parm.Name);
         Assert.AreEqual(typeName, parm.Type.Name);
         Assert.AreEqual(isOut, parm.IsOut);
         Assert.AreEqual(isRef, parm.IsRef);
         Assert.AreEqual(isParamArray, parm.IsParamArray);
         Assert.AreEqual(isOptional, parm.IsOptional);
         Assert.AreEqual(isArray, parm.Type.IsArray);
         Assert.AreEqual(ordinal, parm.Ordinal);
         Assert.AreEqual(defaultValue, parm.DefaultValue);
         Assert.AreEqual(defaultConstantIdentifier, parm.DefaultConstantIdentifier);
      }
      #endregion

      #region returned type tests
      [TestMethod, TestCategory(ReturnTypeNameCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
         var methods = root.Classes.First().Methods.ToArray();
         Assert.AreEqual("System.String", methods[0].ReturnType.QualifiedName);
         Assert.AreEqual("System.Int32", methods[1].ReturnType.QualifiedName);
         Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", methods[2].ReturnType.QualifiedName);
         Assert.AreEqual("BadName", methods[3].ReturnType.Name);
      }

      [TestMethod, TestCategory(ReturnTypeNameCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
         var properties = root.Classes.First().Properties.ToArray();
         Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", properties[2].PropertyType.QualifiedName);
         Assert.AreEqual("BadName", properties[3].PropertyType.Name);
         Assert.AreEqual("Foo4", properties[3].Name);
      }

      [TestMethod, TestCategory(ReturnTypeNameCategory)]
      public void Can_get_return_type_name_for_field()
      {
         var csharpCode = @"
using System
public class Foo  
{
   public string Foo1;
   public int Foo2;
   public System.Diagnostics.Tracing.EventKeyword Foo3;
   public BadName Foo4;
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var fields = root.Classes.First().Fields.ToArray();
         Assert.AreEqual("String", fields[0].ReturnType.Name);
         Assert.AreEqual("Int32", fields[1].ReturnType.Name);
         Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", fields[2].ReturnType.QualifiedName);
         Assert.AreEqual("BadName", fields[3].ReturnType.Name);
      }

      [TestMethod, TestCategory(ReturnTypeNameCategory)]
      public void Can_get_return_type_name_for_parameter()
      {
         var csharpCode = @"
using System
public class Foo  
{
   public void Foo3(string s, int i, System.Diagnostics.Tracing.EventKeyword keyword, BadName whatever){}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var parameters = root.Classes.First().Methods.First().Parameters.ToArray();
         Assert.AreEqual("String", parameters[0].Type.Name);
         Assert.AreEqual("Int32", parameters[1].Type.Name);
         Assert.AreEqual("System.Diagnostics.Tracing.EventKeyword", parameters[2].Type.QualifiedName);
         Assert.AreEqual("BadName", parameters[3].Type.Name);
      }
      #endregion

      #region namespace tests
      [TestMethod, TestCategory(NamespaceCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
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

      [TestMethod, TestCategory(NamespaceCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
         var namespaces = root.ChildNamespaces;
         var allNamespaces = root.DescendantNamespaces;
         var nonEmptyNamespaces = root.Namespaces;
         Assert.AreEqual(1, namespaces.Count());
         Assert.AreEqual(4, allNamespaces.Count());
         Assert.AreEqual(3, allNamespaces.First().DescendantNamespaces.Count());
         Assert.AreEqual(0, nonEmptyNamespaces.Count());
      }

      [TestMethod, TestCategory(NamespaceCategory)]
      public void Does_not_crash_on_no_namespaces_from_root()
      {
         var csharpCode = @"
public enum Foo1 : byte {}
";
         var root = RDom.CSharp.Load(csharpCode);
         var namespaces = root.ChildNamespaces;
         var allNamespaces = root.DescendantNamespaces;
         var nonEmptyNamespaces = root.Namespaces;
         Assert.AreEqual(0, namespaces.Count());
         Assert.AreEqual(0, allNamespaces.Count());
         Assert.AreEqual(0, nonEmptyNamespaces.Count());
      }

      [TestMethod, TestCategory(NamespaceCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
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

      [TestMethod, TestCategory(NamespaceCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
         var topNamespace = root.DescendantNamespaces.First();
         var namespaces = topNamespace.ChildNamespaces;
         var allNamespaces = topNamespace.DescendantNamespaces;
         var nonEmptyNamespaces = topNamespace.Namespaces;
         Assert.AreEqual(2, namespaces.Count());
         Assert.AreEqual(3, allNamespaces.Count());
         Assert.AreEqual(1, allNamespaces.First().ChildNamespaces.Count());
         Assert.AreEqual(0, nonEmptyNamespaces.Count());
      }

      [TestMethod, TestCategory(NamespaceCategory)]
      public void Does_not_crash_on_no_namespaces_from_namespace()
      {
         var csharpCode = @"
namespace Namespace1
{
   public enum Foo1 : byte {}
}
";
         var root = RDom.CSharp.Load(csharpCode);
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
      [TestMethod, TestCategory(MemberKindCategory)]
      public void Can_get_member_kind_for_members()
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
         var root = RDom.CSharp.Load(csharpCode);
         var members = root.Classes.First().Members.ToArray();
         Assert.AreEqual(MemberKind.Property, members[0].MemberKind);
         Assert.AreEqual(MemberKind.Method, members[1].MemberKind);
         Assert.AreEqual(MemberKind.Field, members[2].MemberKind);
         Assert.AreEqual(MemberKind.Class, members[3].MemberKind);
         Assert.AreEqual(MemberKind.Structure, members[4].MemberKind);
         Assert.AreEqual(MemberKind.Interface, members[5].MemberKind);
         Assert.AreEqual(MemberKind.Enum, members[6].MemberKind);
      }

      [TestMethod, TestCategory(MemberKindCategory)]
      public void Can_get_member_kind_for_members_via_requestValue()
      {
         var csharpCode = @"
using System
public class Foo  
{
   public Foo() {}
   public string Foo1{get; set;}
   public Int32 Foo2(){}
   public string Foo3;
   public class Foo4{}
   public struct Foo5{}
   public interface Foo6{}
   public enum Foo7{}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var members = root.Classes.First().Members.ToArray();
         Assert.AreEqual(MemberKind.Constructor, members[0].RequestValue("MemberKind"));
         Assert.AreEqual(MemberKind.Property, members[1].RequestValue("MemberKind"));
         Assert.AreEqual(MemberKind.Method, members[2].RequestValue("MemberKind"));
         Assert.AreEqual(MemberKind.Field, members[3].RequestValue("MemberKind"));
         Assert.AreEqual(MemberKind.Class, members[4].RequestValue("MemberKind"));
         Assert.AreEqual(MemberKind.Structure, members[5].RequestValue("MemberKind"));
         Assert.AreEqual(MemberKind.Interface, members[6].RequestValue("MemberKind"));
         Assert.AreEqual(MemberKind.Enum, members[7].RequestValue("MemberKind"));
      }

      [TestMethod, TestCategory(MemberKindCategory)]
      public void RequestValue_returns_null_if_property_not_found()
      {
         var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var members = root.Classes.First().Members.ToArray();
         Assert.IsNull(members[0].RequestValue("MemberKindX"));
      }

      [TestMethod, TestCategory(MemberKindCategory)]
      public void Can_get_value_from_parameter_via_RequestValue()
      {
         var csharpCode = @"
using System
public class Foo  
{
   public string Foo1(int Bar) {}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var parameters = root.Classes.First().Methods.First().Parameters.ToArray();
         Assert.AreEqual(false, (bool)parameters[0].RequestValue("IsOut"));
      }
      #endregion

      #region property access
      [TestMethod, TestCategory(PropertyAccessCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
         var properties = root.Classes.First().Properties.ToArray();
         Assert.IsTrue(properties[0].CanGet);
         Assert.IsTrue(properties[0].CanSet);
         Assert.IsTrue(properties[1].CanGet);
         Assert.IsFalse(properties[1].CanSet);
         Assert.IsFalse(properties[2].CanGet);
         Assert.IsTrue(properties[2].CanSet);
      }

      [TestMethod, TestCategory(PropertyAccessCategory)]
      public void Can_get_property_access_different_accessibility()
      {
         var csharpCode = @"
using System;
public class Foo  
{
   public string Foo1{get; private set;}
   public string Foo2{get; set;}
   public string Foo3{internal get; set; }
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);

         var properties = root.Classes.First().Properties.ToArray();
         VerifyPropertyAccess(properties[0], AccessModifier.Public, AccessModifier.Public, AccessModifier.Private);
         VerifyPropertyAccess(properties[1], AccessModifier.Public, AccessModifier.Public, AccessModifier.Public);
         VerifyPropertyAccess(properties[2], AccessModifier.Public, AccessModifier.Internal, AccessModifier.Public);
         Assert.AreEqual(csharpCode , actual.ToFullString());
      }

      private void VerifyPropertyAccess(IProperty property, AccessModifier propertyAccess,
               AccessModifier getAccess, AccessModifier setAccess)
      {
         Assert.AreEqual(propertyAccess, property.AccessModifier);
         Assert.AreEqual(getAccess, property.GetAccessor.AccessModifier);
         Assert.AreEqual(setAccess, property.SetAccessor.AccessModifier);
      }


      #endregion

      #region miscellaneous
      [TestMethod, TestCategory(MiscellaneousCategory)]
      public void Can_get_class_name_for_class()
      {
         var csharpCode = @"
using System
public class Foo  
{
   public string Foo1{get; set;}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var cl = (RDomClass)root.Classes.First();
         Assert.AreEqual("Foo", cl.Name);
      }

      [TestMethod, TestCategory(MiscellaneousCategory)]
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
         var root = RDom.CSharp.Load(csharpCode);
         var cl = root.RootClasses.ToArray();
         Assert.AreEqual("Foo", ((RDomClass)cl[0]). Name);
         Assert.AreEqual("Foo1", ((RDomClass)cl[1]).Name);
         Assert.AreEqual("Foo2", ((RDomClass)cl[2]).Name);
      }

      #endregion

      private IEnumerable<IHasAccessModifier> VerifyAccessModifier(string csharpCode,
                  Func<IRoot, IEnumerable<IHasAccessModifier>> getItems, params AccessModifier[] modifiers)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var items = getItems(root);
         for (int i = 0; i < items.Count(); i++)
         { Assert.AreEqual(modifiers[i], items.ElementAt(i).AccessModifier); }
         Assert.AreEqual(csharpCode, actual.ToFullString());
         return items;
      }

      private IHasReturnType VerifyType(string csharpCode, Func<IRoot, IDom> getItem, string fullName)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var item = getItem(root) as IHasReturnType;
         var retType = item.ReturnType;
         Assert.IsNotNull(retType);
         var name = fullName.SubstringAfterLast(".");
         var ns = fullName.SubstringBefore(".");
         Assert.AreEqual(name, retType.Name, "Name");
         Assert.AreEqual(fullName, retType.QualifiedName, "QualifiedName");
         Assert.AreEqual(ns, retType.Namespace, "Namespace");
         Assert.AreEqual(csharpCode, actual.ToFullString());
         return item;
      }

      private IEvent VerifyEvent(string csharpCode, Func<IRoot, IEvent> getItem, string fullName)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var item = getItem(root);
         var type = item.Type;
         Assert.IsNotNull(type);
         var name = fullName.Contains(".") ? fullName.SubstringAfterLast(".") : fullName;
         var ns = fullName.SubstringBefore(".");
         Assert.AreEqual(name, type.Name, "Name");
         Assert.AreEqual(fullName, type.QualifiedName, "QualifiedName");
         Assert.AreEqual(ns, type.Namespace, "Namespace");
         Assert.AreEqual(csharpCode, actual.ToFullString());
         return item;
      }


      private void VerifyOOTypeMember<T>(string csharpCode, Func<IRoot, IEnumerable<T>> getItems,
                   params Tuple<bool, bool, bool, bool, bool, bool>[] expectedValues)
          where T : IOOTypeMember, ICanBeNew, ICanBeStatic
      {
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var items = getItems(root);
         for (int i = 0; i < items.Count(); i++)
         {
            Assert.AreEqual(expectedValues[i].Item1, items.ElementAt(i).IsVirtual);
            Assert.AreEqual(expectedValues[i].Item2, items.ElementAt(i).IsAbstract);
            Assert.AreEqual(expectedValues[i].Item3, items.ElementAt(i).IsOverride);
            Assert.AreEqual(expectedValues[i].Item4, items.ElementAt(i).IsSealed);
            Assert.AreEqual(expectedValues[i].Item5, items.ElementAt(i).IsNew);
            Assert.AreEqual(expectedValues[i].Item6, items.ElementAt(i).IsStatic);
         }
         Assert.AreEqual(csharpCode, actual.ToFullString());
      }

      private void VerifyStatic<T>(string csharpCode, Func<IRoot, IEnumerable<T>> getItems,
                   params Tuple<bool>[] expectedValues)
          where T : ICanBeStatic
      {
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var items = getItems(root);
         for (int i = 0; i < items.Count(); i++)
         {
            Assert.AreEqual(expectedValues[i].Item1, items.ElementAt(i).IsStatic);
         }
         Assert.AreEqual(csharpCode, actual.ToFullString());
      }

      private void VerifyOOClass(string csharpCode, Func<IRoot, IEnumerable<IClass>> getItems,
           params Tuple<bool, bool, bool>[] expectedValues)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var items = getItems(root);
         for (int i = 0; i < items.Count(); i++)
         {
            Assert.AreEqual(expectedValues[i].Item1, items.ElementAt(i).IsAbstract);
            Assert.AreEqual(expectedValues[i].Item2, items.ElementAt(i).IsSealed);
            Assert.AreEqual(expectedValues[i].Item3, items.ElementAt(i).IsStatic);
         }
         Assert.AreEqual(csharpCode, actual.ToFullString());
      }

      private void VerifyOOField(string csharpCode, Func<IRoot, IEnumerable<IField>> getItems,
           params Tuple<bool>[] expectedValues)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var items = getItems(root);
         for (int i = 0; i < items.Count(); i++)
         {
            Assert.AreEqual(expectedValues[i].Item1, items.ElementAt(i).IsStatic);
         }
         Assert.AreEqual(csharpCode, actual.ToFullString());
      }
   }
}
