using System;
using System.Collections.Generic;
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
      private const string ManipulateAttributesCategory = "ManipulateAttributes";

      #region get attributes
      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_class()
      {
         var csharpCode = @"
                        [Serializable]                        
                        public class MyClass
                            { }";

         VerifyAttributes(csharpCode, root => root.Classes.First().Attributes, 1,
             false, "Serializable");
      }

      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_enum()
      {
         var csharpCode = @"
                        [Serializable]                        
                        public enum MyEnum
                            { }";
         VerifyAttributes(csharpCode, root => root.Enums.First().Attributes, 1,
             false, "Serializable");
      }

      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_struct()
      {
         var csharpCode = @"
                        [Serializable]                        
                        public struct MyStruct
                            { }";
         VerifyAttributes(csharpCode, root => root.Structures.First().Attributes, 1,
             false, "Serializable");
      }

      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_interface()
      {
         var csharpCode = @"
                        [Serializable]                        
                        public interface MyInterface
                            { }";
         VerifyAttributes(csharpCode, root => root.Interfaces.First().Attributes, 1,
             false, "Serializable");
      }

      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_method()
      {
         var csharpCode = @"
                        public class MyClass
                        { [Serializable] public int myMethod(int x) { return x; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Attributes, 1,
             false, "Serializable");
      }

      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_parameters()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod([Serializable] int x) { return x; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Parameters.First().Attributes, 1,
             false, "Serializable");
      }

      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_property()
      {
         var csharpCode = @"
                        public class MyClass
                        { [Serializable]  public int myProperty { get; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Properties.First().Attributes, 1,
             false, "Serializable");
      }

      [TestMethod, TestCategory(SimpleAttributeCategory)]
      public void Can_get_attributes_on_field()
      {
         var csharpCode = @"
                        public class MyClass
                        { [Serializable]  public int myField; }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Fields.First().Attributes, 1,
             false, "Serializable");
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
                            { }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Attributes, 2,
             false, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
      public void Can_get_multiple_attributes_in_separate_brackets_on_enum()
      {
         var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public enum MyEnum
                            { }";
         VerifyAttributes(csharpCode, root => root.Enums.First().Attributes, 2,
             false, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
      public void Can_get_multiple_attributes_in_separate_brackets_on_struct()
      {
         var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public struct MyStruct
                            { }";
         VerifyAttributes(csharpCode, root => root.Structures.First().Attributes, 2,
             false, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
      public void Can_get_multiple_attributes_in_separate_brackets_on_interface()
      {
         var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public interface MyInterface
                            { }";
         VerifyAttributes(csharpCode, root => root.Interfaces.First().Attributes, 2,
             false, "Serializable", "TestClass");
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
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Attributes, 2,
             false, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
      public void Can_get_multiple_attributes_in_separate_brackets_on_parameter()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod([Serializable][TestClass] int x) { return x; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Parameters.First().Attributes, 2,
             false, "Serializable", "TestClass");
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
         VerifyAttributes(csharpCode, root => root.Classes.First().Properties.First().Attributes, 2,
             false, "Serializable", "TestClass");
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
         VerifyAttributes(csharpCode, root => root.Classes.First().Fields.First().Attributes, 2,
             false, "Serializable", "TestClass");
      }

      #endregion

      #region get multiple attributes many in bracket
      [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_class()
      {
         var csharpCode = @"
                        [Serializable, TestClass]                        
                        public class MyClass
                            { }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_enum()
      {
         var csharpCode = @"
                        [Serializable, TestClass]                        
                        public enum MyEnum
                            { }";
         VerifyAttributes(csharpCode, root => root.Enums.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_struct()
      {
         var csharpCode = @"
                        [Serializable, TestClass]                        
                        public struct MyStruct
                            { }";
         VerifyAttributes(csharpCode, root => root.Structures.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_interface()
      {
         var csharpCode = @"
                        [Serializable, TestClass]                        
                        public interface MyInterface
                            { }";
         VerifyAttributes(csharpCode, root => root.Interfaces.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_method()
      {
         var csharpCode = @"
                         public class MyClass
                        { 
                        [Serializable, TestClass]                        
                          public int myMethod(int x) { return x; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_parameter()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod([Serializable, TestClass] int x) { return x; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Parameters.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_property()
      {
         var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable, TestClass]                        
                         public int myProperty { get; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Properties.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }

      [TestMethod, TestCategory(AttributesCombinedInBracketsCategory)]
      public void Can_get_multiple_attributes_in_shared_brackets_on_field()
      {
         var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable, TestClass]                        
                        public int myField;  }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Fields.First().Attributes, 2,
             true, "Serializable", "TestClass");
      }
      #endregion

      #region get multiple attributes with mixed bracketing
      [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_class()
      {
         var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public class MyClass
                            { }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
      }

      [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_enum()
      {
         var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public enum MyEnum
                            { }";
         VerifyAttributes(csharpCode, root => root.Enums.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
      }

      [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_struct()
      {
         var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public struct MyStruct
                            { }";
         VerifyAttributes(csharpCode, root => root.Structures.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
      }

      [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_interface()
      {
         var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public interface MyInterface
                            { }";
         VerifyAttributes(csharpCode, root => root.Interfaces.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
      }

      [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_method()
      {
         var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myMethod(int x) { return x; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
      }

      [TestMethod, TestCategory(SeparateBracketsAttributeCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_on_parameter()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod([Serializable, TestClass] [Ignore] int x) { return x; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Parameters.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
      }

      [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_property()
      {
         var csharpCode = @"
                        public class MyClass
                       { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myProperty { get; } }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Properties.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
      }

      [TestMethod, TestCategory(AttributesMixedBracketingCategory)]
      public void Can_get_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_field()
      {
         var csharpCode = @"
                        public class MyClass
                       { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myField;  }";
         VerifyAttributes(csharpCode, root => root.Classes.First().Fields.First().Attributes, 3,
             true, "Serializable", "TestClass", "Ignore");
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
                            { }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Attributes,
             3,
             false, "LocalizationResources", "Name", "SemanticLog")
                     .ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 3)
                     .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: "Fred", kind: LiteralKind.String);
         VerifyAttributeValue(attributeValues[1], name: "", value: "Joe", kind: LiteralKind.String);
         VerifyAttributeValue(attributeValues[2], name: "Cats", value: 42, kind: LiteralKind.Numeric);


         attributeValues = VerifyAttributeValues(attributes[1], count: 1)
                     .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: "KadGen-Test-Temp", kind: LiteralKind.String);
         VerifyAttributeValues(attributes[2], count: 0);
      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_attribute_values_of_most_types_on_class_with_equals()
      {
         var csharpCode = @"
                        [Test(Int=42, Bool=true, Double=3.14, StringTest = ""Foo"")]
                        public class MyClass
                            { }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Attributes,
             1,
             false, "Test")
                     .ToArray();

         var attributeValues = VerifyAttributeValues(attributes[0], count: 4)
                     .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "Int", value: 42, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "Bool", value: true, kind: LiteralKind.Boolean);
         VerifyAttributeValue(attributeValues[2], name: "Double", value: 3.14, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[3], name: "StringTest", value: "Foo", kind: LiteralKind.String);

      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_attribute_values_of_most_types_on_class_with_colons()
      {
         var csharpCode = @"
                        [Test(Int:42, Bool : true, Double :3.14, StringTest: ""Foo"")]
                        public class MyClass
                            { }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Attributes,
             1,
             false, "Test")
                    .ToArray();

         var attributeValues = VerifyAttributeValues(attributes[0], count: 4)
                     .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "Int", value: 42, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "Bool", value: true, kind: LiteralKind.Boolean);
         VerifyAttributeValue(attributeValues[2], name: "Double", value: 3.14, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[3], name: "StringTest", value: "Foo", kind: LiteralKind.String);

      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_attribute_value_of_typeof_primitive_on_class()
      {
         var csharpCode = @"
                        [Test(TypeTest = typeof(string))]
                        public class MyClass
                            { }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Attributes,
             1,
             false, "Test")
                     .ToArray();
         var current = VerifyAttributeValues(attributes[0], count: 1)
                .First();
         VerifyTypeOfAttributeValue(current, name: "String");
      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_attribute_value_of_typeof_identifier_only_on_class()
      {
         var csharpCode = @"
                        [Test(TypeTest = typeof(Foo))]
                        public class MyClass
                            { }";

         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Attributes,
             1,
             false, "Test")
                    .ToArray();
         var current = VerifyAttributeValues(attributes[0], count: 1)
                .First();
         VerifyTypeOfAttributeValue(current, name: "Foo");
      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_attribute_value_of_typeof_referenced_on_class()
      {
         var csharpCode = @"
                        [Test(TypeTest = typeof(DateTime))]
                        public class MyClass
                            { }";

         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Attributes,
             1,
             false, "Test")
               .ToArray();
         var current = VerifyAttributeValues(attributes[0], count: 1)
                .First();
         VerifyTypeOfAttributeValue(current, name: "DateTime");
      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_simple_attribute_values_on_class()
      {
         var csharpCode = @"
                        [Version(2)]
                        [Something(3, true)]
                        public class MyClass
                            { }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Attributes,
             2,
             false, "Version", "Something")
                     .ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 1)
                .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 2, kind: LiteralKind.Numeric);

         attributeValues = VerifyAttributeValues(attributes[1], count: 2)
              .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 3, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "", value: true, kind: LiteralKind.Boolean);
      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_simple_attribute_values_on_structure()
      {
         var csharpCode = @"
                        [Version(2)]
                        [Something(3, true)]
                        public struct MyStructure
                            { }";
         var attributes = VerifyAttributes(csharpCode, root => root.Structures.First().Attributes,
             2,
             false, "Version", "Something")
                   .ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 1)
                .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 2, kind: LiteralKind.Numeric);

         attributeValues = VerifyAttributeValues(attributes[1], count: 2)
              .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 3, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "", value: true, kind: LiteralKind.Boolean);
      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_simple_attribute_values_on_enum()
      {
         var csharpCode = @"
                        [Version(2)]
                        [Something(3, true)]
                        public enum MyEnum
                            { }";
         var attributes = VerifyAttributes(csharpCode, root => root.Enums.First().Attributes,
             2,
             false, "Version", "Something")
                  .ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 1)
                .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 2, kind: LiteralKind.Numeric);

         attributeValues = VerifyAttributeValues(attributes[1], count: 2)
              .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 3, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "", value: true, kind: LiteralKind.Boolean);

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
                        }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Methods.First().Attributes,
             2,
             false, "Version", "Something")
                     .ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 1)
                .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 2, kind: LiteralKind.Numeric);

         attributeValues = VerifyAttributeValues(attributes[1], count: 2)
              .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 3, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "", value: true, kind: LiteralKind.Boolean);
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
                        }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Properties.First().Attributes,
             2,
             false, "Version", "Something")
                .ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 1)
                .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 2, kind: LiteralKind.Numeric);

         attributeValues = VerifyAttributeValues(attributes[1], count: 2)
              .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 3, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "", value: true, kind: LiteralKind.Boolean);

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
                        }";
         var attributes = VerifyAttributes(csharpCode, root => root.Classes.First().Fields.First().Attributes,
             2,
             false, "Version", "Something")
                  .ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 1)
                .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 2, kind: LiteralKind.Numeric);

         attributeValues = VerifyAttributeValues(attributes[1], count: 2)
              .ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: 3, kind: LiteralKind.Numeric);
         VerifyAttributeValue(attributeValues[1], name: "", value: true, kind: LiteralKind.Boolean);

      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_expression_attribute_value()
      {
         var source = @"
using System;

namespace Test
{
    public class ContractNamespaceAttribute : Attribute
    {
        public ContractNamespaceAttribute(string text)
        {
        }

        public Type MyType {get; set;}
    }

    public class Const
    {
        public const string Test = ""TestContract"";
    }

    [ContractNamespace(Const.Test)]
    class TestClass
    {
    }
}";
         var attributes = VerifyAttributes(source,
                  root => root.RootClasses.First(x => x.Name == "TestClass").Attributes,
                  1, false, "ContractNamespace").ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 1).ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: "TestContract", kind: LiteralKind.Constant);

      }

      [TestMethod, TestCategory(AttributeValuesCategory)]
      public void Can_get_multiline_attribute_value()
      {
         // One test because I'm testing whitespace value with different order of named attribute values
         var source = @"
using System;

namespace Test
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(""Microsoft.Naming"", 
        ""CA1711:IdentifiersShouldNotHaveIncorrectSuffix"",
                Justification = ""Because"")]
    public class MyClass
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(""Microsoft.Naming"", 
           Justification = ""Just because"",
              A: ""CA1711:IdentifiersShouldNotHaveIncorrectSuffix"")]
        public void Whatever(string text)
        {}
    }
}";
         var attributes = VerifyAttributes(source,
                  root => root.RootClasses.First().Attributes,
                  1, false, "System.Diagnostics.CodeAnalysis.SuppressMessage").ToArray();
         var attributeValues = VerifyAttributeValues(attributes[0], count: 3).ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: "Microsoft.Naming", kind: LiteralKind.String);
         VerifyAttributeValue(attributeValues[1], name: "", value: "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", kind: LiteralKind.String);
         VerifyAttributeValue(attributeValues[2], name: "Justification", value: "Because", kind: LiteralKind.String);

         attributes = VerifyAttributes(source,
                  root => root.RootClasses.First().Methods.First().Attributes,
                  1, false, "System.Diagnostics.CodeAnalysis.SuppressMessage").ToArray();
         attributeValues = VerifyAttributeValues(attributes[0], count: 3).ToArray();
         VerifyAttributeValue(attributeValues[0], name: "", value: "Microsoft.Naming", kind: LiteralKind.String);
         VerifyAttributeValue(attributeValues[1], name: "Justification", value: "Just because", kind: LiteralKind.String);
         VerifyAttributeValue(attributeValues[2], name: "A", value: "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", kind: LiteralKind.String);

      }

      #endregion

      #region get root class attributes


      #endregion

      #region manipulate attributes
      [TestMethod, TestCategory(ManipulateAttributesCategory)]
      public void Can_remove_attribute_on_property()
      {
         var csharpCode = @"
                        public class MyClass
                       { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myProperty { get; } }";
         var csharpCodeChanged = csharpCode.Replace(", TestClass", "");
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCodeChanged);
         var syntax2 = RDomCSharp.Factory.BuildSyntax(root2).ToFullString();

         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var prop = root.Classes.First().Properties.First();
         Assert.AreEqual(3, prop.Attributes.Count());
         var attr = prop.Attributes.ElementAt(1);
         prop.Attributes.RemoveAttribute(attr);
         Assert.AreEqual(2, prop.Attributes.Count());

         var syntax = RDomCSharp.Factory.BuildSyntax(root).ToFullString();
         Assert.AreEqual(syntax2, syntax);
      }

      [TestMethod, TestCategory(ManipulateAttributesCategory)]
      public void Can_get_remove_attribute_values_on_field()
      {
         var csharpCode = @"
                        public class MyClass
                        { 
                            [Version(2)]
                            [Something(3, true)]
                            public string foo;
                        }";
         var csharpCodeChanged = csharpCode.Replace(", true", "");
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCodeChanged);
         var syntax2 = RDomCSharp.Factory.BuildSyntax(root2).ToFullString();

         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var field = root.Classes.First().Fields.First();
         var attr = field.Attributes.ElementAt(1);
         Assert.AreEqual(2, field.Attributes.Count());
         Assert.AreEqual(2, attr.AttributeValues.Count());
         var attrValue = attr.AttributeValues.ElementAt(1);
         attr.RemoveAttributeValue(attrValue);
         Assert.AreEqual(2, field.Attributes.Count());
         Assert.AreEqual(1, attr.AttributeValues.Count());

         var syntax = RDomCSharp.Factory.BuildSyntax(root).ToFullString();
         Assert.AreEqual(syntax2, syntax);
      }

      #endregion

      private static IEnumerable<IAttribute> VerifyAttributes(string csharpCode,
          Func<IRoot, IEnumerable<IAttribute>> makeAttributes,
          int count, bool skipBuildSyntaxCheck, params string[] names)
      {
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var attributes = makeAttributes(root).ToArray();
         Assert.AreEqual(count, attributes.Count());
         for (int i = 0; i < attributes.Count(); i++)
         {
            Assert.AreEqual(names[i], attributes[i].Name);
         }
         if (!skipBuildSyntaxCheck)
         {
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
         }
         return attributes;
      }

      private IEnumerable<IAttributeValue> VerifyAttributeValues(IAttribute attribute, int count)
      {
         var attributeValues = attribute.AttributeValues.ToArray();
         Assert.AreEqual(count, attributeValues.Count());
         return attributeValues;
      }

      private void VerifyAttributeValue(IAttributeValue attributeValue, string name, object value, LiteralKind kind)
      {
         Assert.AreEqual(name, attributeValue.Name);
         Assert.AreEqual(value, attributeValue.Value);
         Assert.AreEqual(kind, attributeValue.ValueType);
      }

      private static void VerifyTypeOfAttributeValue(IAttributeValue current, string name)
      {
         Assert.AreEqual(LiteralKind.Type, current.ValueType);
         var refType = current.Value as RDomReferencedType;
         Assert.IsNotNull(refType);
         Assert.AreEqual(name, refType.Name);
      }


   }
}
