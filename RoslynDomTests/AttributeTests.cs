using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class AttributeTests
    {
        private const string SimpleAttributeCategory = "AttributesSimple";
        private const string SeparateBracketsAttributeCategory = "AttributesSeparateBracket";
        private const string AttributesCombinedInBracketsCategory = "AttributesCombinedBracket";
        private const string AttributesMixedBracketingCategory = "AttributesMixedBracketing";
        private const string AttributeValuesCategory = "AttributesValues";
        private const string RootClassAttributesCategory = "RootClassAttributes";

        #region get attributes
        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_class()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.Classes.First();
            var attributes = class1.Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }

        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_enum()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }

        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_struct()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Structures.First().Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }

        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_interface()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }

        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { [Serializable] public int myMethod(int x) { return x; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }

        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_parameters()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myMethod([Serializable] int x) { return x; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Parameters.First().Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }

        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { [Serializable]  public int myProperty { get; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }

        [TestMethod, TestCategory(SimpleAttributeCategory)]
        public void Can_get_attributes_on_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { [Serializable]  public int myField; }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Fields.First().Attributes;
            Assert.AreEqual(1, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
        }


        #endregion

        #region get multiple attributes one to a bracket set
        [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
        public void Can_get_multiple_attributes_in_separate_brackets_on_class()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
        public void Can_get_multiple_attributes_in_separate_brackets_on_enum()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
        public void Can_get_multiple_attributes_in_separate_brackets_on_struct()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Structures.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
        public void Can_get_multiple_attributes_in_separate_brackets_on_interface()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
        public void Can_get_multiple_attributes_in_separate_brackets_on_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                        public int myMethod(int x) { return x; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
        public void Can_get_multiple_attributes_in_separate_brackets_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                        public int myProperty { get; }  }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
        public void Can_get_multiple_attributes_in_separate_brackets_on_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                        public int myField;  }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Fields.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        #endregion

        #region get multiple attributes many in bracket
        [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
        public void Can_get_multiple_attributes_in_shared_brackets_on_class()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
        public void Can_get_multiple_attributes_in_shared_brackets_on_enum()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
        public void Can_get_multiple_attributes_in_shared_brackets_on_struct()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Structures.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
        public void Can_get_multiple_attributes_in_shared_brackets_on_interface()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
        public void Can_get_multiple_attributes_in_shared_brackets_on_method()
        {
            var csharpCode = @"
                         public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                          public int myMethod(int x) { return x; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
        public void Can_get_multiple_attributes_in_shared_brackets_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                         public int myProperty { get; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
        public void Can_get_multiple_attributes_in_shared_brackets_on_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                        public int myField;  }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Fields.First().Attributes;
            Assert.AreEqual(2, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Last().Name);
        }
        #endregion

        #region get multiple attributes with mixed bracketing
        [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_class()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_enum()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_struct()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Structures.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_interface()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myMethod(int x) { return x; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                       { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myProperty { get; } }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_field()
        {
            var csharpCode = @"
                        public class MyClass
                       { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myField;  }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Fields.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            Assert.AreEqual("Serializable", attributes.Attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Attributes.Last().Name);
        }
        #endregion

        #region get attribute values
        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_attribute_values_on_class()
        {
            var csharpCode = @"
                        [LocalizationResources(""Fred"", ""Joe"", Cats=42)]
                        [Name(""KadGen-Test-Temp"")]
                        [SemanticLog]
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(3, attributes.Attributes.Count());
            var first = attributes.Attributes.First();
            Assert.AreEqual("LocalizationResources", first.Name);
            Assert.AreEqual(3, first.AttributeValues.Count());
            var current = first.AttributeValues.First();
            Assert.AreEqual("", current.Name);
            Assert.AreEqual("Fred", current.Value);
            Assert.AreEqual(LiteralKind.String, current.ValueType );
             current = first.AttributeValues.Skip(1).First();
            Assert.AreEqual("", current.Name);
            Assert.AreEqual("Joe", current.Value);
            Assert.AreEqual(LiteralKind.String, current.ValueType);
             current = first.AttributeValues.Last();
            Assert.AreEqual("Cats", current.Name);
            Assert.AreEqual(42, current.Value);
            Assert.AreEqual(LiteralKind.Numeric, current.ValueType);

            Assert.AreEqual("Name", attributes.Attributes.Skip(1).First().Name);
            Assert.AreEqual("SemanticLog", attributes.Attributes.Last().Name);
        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_attribute_values_of_most_types_on_class_with_equals()
        {
            var csharpCode = @"
                        [Test(Int=42, Bool=true, Double=3.14, StringTest = ""Foo"")]
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributeValues = root.Classes.First().Attributes.Attributes.First().AttributeValues.ToArray() ;
            Assert.AreEqual(4, attributeValues.Count());

            var current = attributeValues[0];
            Assert.AreEqual("Int", current.Name);
            Assert.AreEqual(42, current.Value);
            Assert.AreEqual(LiteralKind.Numeric, current.ValueType);

            current = attributeValues[1];
            Assert.AreEqual("Bool", current.Name);
            Assert.AreEqual(true, current.Value);
            Assert.AreEqual(LiteralKind.Boolean, current.ValueType);

            current = attributeValues[2];
            Assert.AreEqual("Double", current.Name);
            Assert.AreEqual(3.14, current.Value);
            Assert.AreEqual(LiteralKind.Numeric, current.ValueType);

            current = attributeValues[3];
            Assert.AreEqual("StringTest", current.Name);
            Assert.AreEqual("Foo", current.Value);
            Assert.AreEqual(LiteralKind.String, current.ValueType);

            //current = attributeValues[4];
            //Assert.AreEqual("TypeTest", current.Name);
            //Assert.AreEqual(stringType, current.Value);
            //Assert.AreEqual(intType, current.ValueType);

        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_attribute_values_of_most_types_on_class_with_colons()
        {
            var csharpCode = @"
                        [Test(Int:42, Bool : true, Double :3.14, StringTest: ""Foo"")]
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributeValues = root.Classes.First().Attributes.Attributes.First().AttributeValues.ToArray();
            Assert.AreEqual(4, attributeValues.Count());

            var current = attributeValues[0];
            Assert.AreEqual("Int", current.Name);
            Assert.AreEqual(42, current.Value);
            Assert.AreEqual(LiteralKind.Numeric, current.ValueType);

            current = attributeValues[1];
            Assert.AreEqual("Bool", current.Name);
            Assert.AreEqual(true, current.Value);
            Assert.AreEqual(LiteralKind.Boolean, current.ValueType);

            current = attributeValues[2];
            Assert.AreEqual("Double", current.Name);
            Assert.AreEqual(3.14, current.Value);
            Assert.AreEqual(LiteralKind.Numeric, current.ValueType);

            current = attributeValues[3];
            Assert.AreEqual("StringTest", current.Name);
            Assert.AreEqual("Foo", current.Value);
            Assert.AreEqual(LiteralKind.String, current.ValueType);

            //current = attributeValues[4];
            //Assert.AreEqual("TypeTest", current.Name);
            //Assert.AreEqual(stringType, current.Value);
            //Assert.AreEqual(intType, current.ValueType);

        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_attribute_value_of_typeof_primitive_on_class()
        {
            var csharpCode = @"
                        [Test(TypeTest = typeof(string))]
                        public class MyClass
                            { }
                        ";

            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributeValues = root.Classes.First().Attributes.Attributes.First().AttributeValues.ToArray() ;
            Assert.AreEqual(1, attributeValues.Count());

            var current = attributeValues[0];
            Assert.AreEqual("TypeTest", current.Name);
            Assert.AreEqual(LiteralKind.Type, current.ValueType);
            var refType = current.Value as RDomReferencedType;
            Assert.IsNotNull(refType);
            Assert.AreEqual("string", refType.Name);

        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_attribute_value_of_typeof_identifier_only_on_class()
        {
            var csharpCode = @"
                        [Test(TypeTest = typeof(Foo))]
                        public class MyClass
                            { }
                        ";

            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributeValues = root.Classes.First().Attributes.Attributes.First().AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());

            var current = attributeValues[0];
            Assert.AreEqual("TypeTest", current.Name);
            Assert.AreEqual(LiteralKind.Type, current.ValueType);
            var refType = current.Value as RDomReferencedType;
            Assert.IsNotNull(refType);
            Assert.AreEqual("Foo", refType.Name);
        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_attribute_value_of_typeof_referenced_on_class()
        {
            var csharpCode = @"
                        [Test(TypeTest = typeof(DateTime))]
                        public class MyClass
                            { }
                        ";

            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var attributeValues = root.Classes.First().Attributes.Attributes.First().AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());

            var current = attributeValues[0];
            Assert.AreEqual("TypeTest", current.Name);
            Assert.AreEqual(LiteralKind.Type, current.ValueType);
            var refType = current.Value as RDomReferencedType;
            Assert.IsNotNull(refType);
            Assert.AreEqual("DateTime", refType.Name);

        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_simple_attribute_values_on_class()
        {
            var csharpCode = @"
                        [Version(2)]
                        [Something(3, true)]
                        public class MyClass
                        {}
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.Classes.First();
            var attributes = cl.Attributes.Attributes.ToArray();
            var attributeValues = attributes[0].AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());
            Assert.AreEqual(2, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);

            attributeValues = attributes[1].AttributeValues.ToArray();
            Assert.AreEqual(2, attributeValues.Count());
            Assert.AreEqual(3, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);
            Assert.AreEqual(true, attributeValues[1].Value);
            Assert.AreEqual("", attributeValues[1].Name);
        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_simple_attribute_values_on_structure()
        {
            var csharpCode = @"
                        [Version(2)]
                        [Something(3, true)]
                        public struct MyStructure
                        { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var structure = root.Structures .First();
            var attributes = structure.Attributes.Attributes.ToArray();
            var attributeValues = attributes[0].AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());
            Assert.AreEqual(2, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);

            attributeValues = attributes[1].AttributeValues.ToArray();
            Assert.AreEqual(2, attributeValues.Count());
            Assert.AreEqual(3, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);
            Assert.AreEqual(true, attributeValues[1].Value);
            Assert.AreEqual("", attributeValues[1].Name);
        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_simple_attribute_values_on_enum()
        {
            var csharpCode = @"
                        [Version(2)]
                        [Something(3, true)]
                        public enum MyEnum
                        { }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var myEnum = root.Enums.First();
            var attributes = myEnum.Attributes.Attributes.ToArray();
            var attributeValues = attributes[0].AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());
            Assert.AreEqual(2, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);

            attributeValues = attributes[1].AttributeValues.ToArray();
            Assert.AreEqual(2, attributeValues.Count());
            Assert.AreEqual(3, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);
            Assert.AreEqual(true, attributeValues[1].Value);
            Assert.AreEqual("", attributeValues[1].Name);

        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_simple_attribute_values_on_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                            [Version(2)]
                            [Something(3, true)]
                            public void foo(int Bar, string Bar2) { }
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var method = root.Classes.First().Methods.First();
            var attributes = method.Attributes.Attributes.ToArray();
            var attributeValues = attributes[0].AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());
            Assert.AreEqual(2, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);

            attributeValues = attributes[1].AttributeValues.ToArray();
            Assert.AreEqual(2, attributeValues.Count());
            Assert.AreEqual(3, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);
            Assert.AreEqual(true, attributeValues[1].Value);
            Assert.AreEqual("", attributeValues[1].Name);

        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_simple_attribute_values_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                            [Version(2)]
                            [Something(3, true)]
                            public string foo {get; set; }
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var property = root.Classes.First().Properties .First();
            var attributes = property.Attributes.Attributes.ToArray();
            var attributeValues = attributes[0].AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());
            Assert.AreEqual(2, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);

            attributeValues = attributes[1].AttributeValues.ToArray();
            Assert.AreEqual(2, attributeValues.Count());
            Assert.AreEqual(3, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);
            Assert.AreEqual(true, attributeValues[1].Value);
            Assert.AreEqual("", attributeValues[1].Name);

        }

        [TestMethod, TestCategory(AttributeValuesCategory)]
        public void Can_get_simple_attribute_values_on_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                            [Version(2)]
                            [Something(3, true)]
                            public string foo;
                        }
                        ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var field = root.Classes.First().Fields.First();
            var attributes = field.Attributes.Attributes.ToArray();
            var attributeValues = attributes[0].AttributeValues.ToArray();
            Assert.AreEqual(1, attributeValues.Count());
            Assert.AreEqual(2, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);

            attributeValues = attributes[1].AttributeValues.ToArray();
            Assert.AreEqual(2, attributeValues.Count());
            Assert.AreEqual(3, attributeValues[0].Value);
            Assert.AreEqual("", attributeValues[0].Name);
            Assert.AreEqual(true, attributeValues[1].Value);
            Assert.AreEqual("", attributeValues[1].Name);

        }

        #endregion

        #region get root class attributes
 

        #endregion

    }
}
