using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;
using System.Collections.Generic;
using System;

namespace RoslynDomTests
{

   [TestClass]
   public class GenericsTests
   {
      private const string GenericParamCategory = "GenericParamCountAndName";
      private const string GenericNamingCategory = "GenericNaming";
      private const string GenericVarianceCategory = "GenericVariance";

      #region generic parameter counting
      [TestMethod, TestCategory(GenericParamCategory)]
      public void Can_get_generic_types_for_class()
      {
         var csharpCode = @"
public class Foo0{}
public class Foo1<T>{}
public class Foo2<T, T1>{}
public class Foo3 <T, T1,T2>{}
";
         VerifyTypeParameters(csharpCode, x => x.Classes.ToArray()[0].TypeParameters);
         VerifyTypeParameters(csharpCode, x => x.Classes.ToArray()[1].TypeParameters, "T");
         VerifyTypeParameters(csharpCode, x => x.Classes.ToArray()[2].TypeParameters, "T", "T1");
         VerifyTypeParameters(csharpCode, x => x.Classes.ToArray()[3].TypeParameters, "T", "T1", "T2");
      }

      [TestMethod, TestCategory(GenericParamCategory)]
      public void Can_get_generic_types_for_structure()
      {
         var csharpCode = @"
public struct Foo0{}
public struct Foo1<T>{}
public struct Foo2<T, T1>{}
public struct Foo3<T, T1,T2>{}
";
         //var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         //var structures = root.Structures.ToArray();
         //Assert.AreEqual(0, structures[0].TypeParameters.Count());
         //Assert.AreEqual(1, structures[1].TypeParameters.Count());
         //Assert.AreEqual(2, structures[2].TypeParameters.Count());
         //Assert.AreEqual(3, structures[3].TypeParameters.Count());
         VerifyTypeParameters(csharpCode, x => x.Structures.ToArray()[0].TypeParameters);
         VerifyTypeParameters(csharpCode, x => x.Structures.ToArray()[1].TypeParameters, "T");
         VerifyTypeParameters(csharpCode, x => x.Structures.ToArray()[2].TypeParameters, "T", "T1");
         VerifyTypeParameters(csharpCode, x => x.Structures.ToArray()[3].TypeParameters, "T", "T1", "T2");

      }

      [TestMethod, TestCategory(GenericParamCategory)]
      public void Can_get_generic_types_for_interface()
      {
         var csharpCode = @"
public interface Foo0{}
public interface Foo1<T>{}
public interface Foo2<T, T1>{}
public interface Foo3<T, T1, T2>{}
";
         //var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         //var interfaces = root.Interfaces.ToArray();
         //Assert.AreEqual(0, interfaces[0].TypeParameters.Count());
         //Assert.AreEqual(1, interfaces[1].TypeParameters.Count());
         //Assert.AreEqual(2, interfaces[2].TypeParameters.Count());
         //Assert.AreEqual(3, interfaces[3].TypeParameters.Count());
         VerifyTypeParameters(csharpCode, x => x.Interfaces.ToArray()[0].TypeParameters);
         VerifyTypeParameters(csharpCode, x => x.Interfaces.ToArray()[1].TypeParameters, "T");
         VerifyTypeParameters(csharpCode, x => x.Interfaces.ToArray()[2].TypeParameters, "T", "T1");
         VerifyTypeParameters(csharpCode, x => x.Interfaces.ToArray()[3].TypeParameters, "T", "T1", "T2");
      }

      [TestMethod, TestCategory(GenericParamCategory)]
      public void Can_get_generic_types_for_method()
      {
         var csharpCode = @"
public class Foo
{
public void Foo0(){}
public void Foo1<T>(){}
public void Foo2<T, T1>(){}
public void Foo3<T, T1, T2>(){}
}
";
         //var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         //var methods = root.Classes.First().Methods.ToArray();
         //Assert.AreEqual(0, methods[0].TypeParameters.Count());
         //Assert.AreEqual(1, methods[1].TypeParameters.Count());
         //Assert.AreEqual(2, methods[2].TypeParameters.Count());
         //Assert.AreEqual(3, methods[3].TypeParameters.Count());
         VerifyTypeParameters(csharpCode, x => x.Classes.First().Methods.ToArray()[0].TypeParameters);
         VerifyTypeParameters(csharpCode, x => x.Classes.First().Methods.ToArray()[1].TypeParameters, "T");
         VerifyTypeParameters(csharpCode, x => x.Classes.First().Methods.ToArray()[2].TypeParameters, "T", "T1");
         VerifyTypeParameters(csharpCode, x => x.Classes.First().Methods.ToArray()[3].TypeParameters, "T", "T1", "T2");
      }

      [TestMethod, TestCategory(GenericParamCategory)]
      public void Can_get_generic_return_types_for_method()
      {
         var csharpCode = @"
public class Foo
{
public void Foo0(){}
public List <T>        Foo1(){}
public  List < T, T1>  Foo2(){}
public List<T, T1, T2> Foo3(){}
}
";
         //var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         //var methods = root.Classes.First().Methods.ToArray();
         //Assert.AreEqual(0, methods[0].TypeParameters.Count());
         //Assert.AreEqual(1, methods[1].TypeParameters.Count());
         //Assert.AreEqual(2, methods[2].TypeParameters.Count());
         //Assert.AreEqual(3, methods[3].TypeParameters.Count());
         VerifyTypeArguments(csharpCode, x => x.Classes.First().Methods.ToArray()[0].ReturnType.TypeArguments);
         VerifyTypeArguments(csharpCode, x => x.Classes.First().Methods.ToArray()[1].ReturnType.TypeArguments, "T");
         VerifyTypeArguments(csharpCode, x => x.Classes.First().Methods.ToArray()[2].ReturnType.TypeArguments, "T", "T1");
         VerifyTypeArguments(csharpCode, x => x.Classes.First().Methods.ToArray()[3].ReturnType.TypeArguments, "T", "T1", "T2");
      }
      #endregion

      #region generic variance 
      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_variance_for_generic_parameters_of_classes()
      {
         var csharpCode = @"
public class Foo3<in T, out T1,out in T2, T3>
{}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Classes.First().TypeParameters.ToArray();
         Assert.AreEqual(Variance.In, parameters[0].Variance);
         Assert.AreEqual(Variance.Out, parameters[1].Variance);
         // This is reflects an inconsistency in the Roslyn parsing
         Assert.AreEqual(Variance.Out, parameters[2].Variance);
         Assert.AreEqual(Variance.None, parameters[3].Variance);

      }

      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_variance_for_generic_parameters_of_structures()
      {
         var csharpCode = @"
public struct Foo3<in T, out T1,out in T2, T3>
{}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Structures.First().TypeParameters.ToArray();
         Assert.AreEqual(Variance.In, parameters[0].Variance);
         Assert.AreEqual(Variance.Out, parameters[1].Variance);
         // This is reflects an inconsistency in the Roslyn parsing
         Assert.AreEqual(Variance.Out, parameters[2].Variance);
         Assert.AreEqual(Variance.None, parameters[3].Variance);

      }

      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_variance_for_generic_parameters_of_interfaces()
      {
         var csharpCode = @"
public interface Foo3<in T, out T1,out in T2, T3>
{}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Interfaces.First().TypeParameters.ToArray();
         Assert.AreEqual(Variance.In, parameters[0].Variance);
         Assert.AreEqual(Variance.Out, parameters[1].Variance);
         // This is reflects an inconsistency in the Roslyn parsing
         Assert.AreEqual(Variance.Out, parameters[2].Variance);
         Assert.AreEqual(Variance.None, parameters[3].Variance);

      }

      #endregion

      #region generic constraints 
      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_generic_constraints_for_classes()
      {
         var csharpCode = @"
public class Foo3<in T, out T1, out T2, T3, T4, T5, T6, T7>  
    where T : class
    where T1 : struct
    where T2 : IFoo
    where T4: new()
    where T6 : class, IFoo, IFoo2
    where T7 : T3
{}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Classes.First().TypeParameters.ToArray();
         parameters[0].Check(0, hasReferenceConstraint: true);
         parameters[1].Check(1, hasValueTypeConstraint: true);
         parameters[2].Check(2, constraintCount: 1);
         parameters[3].Check(3);
         parameters[4].Check(4, hasConstructorConstraint: true);
         parameters[5].Check(5);
         parameters[6].Check(6, constraintCount: 2, hasReferenceConstraint: true);
         parameters[7].Check(7, constraintCount: 1);

      }

      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_generic_constraints_for_structures()
      {
         var csharpCode = @"
public struct Foo3<in T, out T1, out T2, T3, T4, T5, T6, T7>  
    where T : class
    where T1 : struct
    where T2 : IFoo
    where T4: new()
    where T6 : class, IFoo, IFoo2
    where T7 : T3
{}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Structures.First().TypeParameters.ToArray();
         parameters[0].Check(0, hasReferenceConstraint: true);
         parameters[1].Check(1, hasValueTypeConstraint: true);
         parameters[2].Check(2, constraintCount: 1);
         parameters[3].Check(3);
         parameters[4].Check(4, hasConstructorConstraint: true);
         parameters[5].Check(5);
         parameters[6].Check(6, constraintCount: 2, hasReferenceConstraint: true);
         parameters[7].Check(7, constraintCount: 1);

      }

      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_generic_constraints_for_interfaces()
      {
         var csharpCode = @"
public interface Foo3<in T, out T1, out T2, T3, T4, T5, T6, T7>  
    where T : class
    where T1 : struct
    where T2 : IFoo
    where T4: new()
    where T6 : class, IFoo, IFoo2
    where T7 : T3
{}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Interfaces.First().TypeParameters.ToArray();
         parameters[0].Check(0, hasReferenceConstraint: true);
         parameters[1].Check(1, hasValueTypeConstraint: true);
         parameters[2].Check(2, constraintCount: 1);
         parameters[3].Check(3);
         parameters[4].Check(4, hasConstructorConstraint: true);
         parameters[5].Check(5);
         parameters[6].Check(6, constraintCount: 2, hasReferenceConstraint: true);
         parameters[7].Check(7, constraintCount: 1);

      }

      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_generic_constraints_for_methods()
      {
         var csharpCode = @"
public class Foo
{
public string Foo3<in T, out T1, out T2, T3, T4, T5, T6, T7>()  
    where T : class
    where T1 : struct
    where T2 : IFoo
    where T4: new()
    where T6 : class, IFoo, IFoo2
    where T7 : T3 {}
{}
}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Classes.First().Methods.First().TypeParameters.ToArray();
         parameters[0].Check(0, hasReferenceConstraint: true);
         parameters[1].Check(1, hasValueTypeConstraint: true);
         parameters[2].Check(2, constraintCount: 1);
         parameters[3].Check(3);
         parameters[4].Check(4, hasConstructorConstraint: true);
         parameters[5].Check(5);
         parameters[6].Check(6, constraintCount: 2, hasReferenceConstraint: true);
         parameters[7].Check(7, constraintCount: 1);

      }

      [TestMethod, TestCategory(GenericVarianceCategory)]
      public void Can_get_generic_constraints_names_for_classes()
      {
         var csharpCode = @"
public class Foo3<T, T1, T2, T3>  
    where T1 : IFoo
    where T2 : class, IFoo, IFoo2
    where T3 : T2
{}
";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var parameters = root.Classes.First().TypeParameters.ToArray();
         IReferencedType[] constraints;
         constraints = parameters[0].ConstraintTypes.ToArray();
         Assert.AreEqual(0, constraints.Count());
         constraints = parameters[1].ConstraintTypes.ToArray();
         Assert.AreEqual("IFoo", constraints[0].Name);
         constraints = parameters[2].ConstraintTypes.ToArray();
         Assert.AreEqual("IFoo", constraints[0].Name);
         Assert.AreEqual("IFoo2", constraints[1].Name);
         constraints = parameters[3].ConstraintTypes.ToArray();
         Assert.AreEqual("T2", constraints[0].Name);
      }



      #endregion

      private static void VerifyTypeParameters(string csharpCode,
               Func<IRoot, IEnumerable<ITypeParameter>> getTypeParams,
                  params string[] names)
      {
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var typeParams = getTypeParams(root).ToArray();
         Assert.AreEqual(names.Length, typeParams.Length);
         for (int i = 0; i < names.Length; i++)
         { Assert.AreEqual(names[i], typeParams[i].Name); }
         var actualString = RDomCSharp.Factory.BuildSyntax(root).ToFullString();
         Assert.AreEqual(csharpCode, actualString);
      }

      private static void VerifyTypeArguments(string csharpCode,
          Func<IRoot, IEnumerable<IReferencedType >> getTypeArgs,
             params string[] names)
      {
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var typeParams = getTypeArgs(root).ToArray();
         Assert.AreEqual(names.Length, typeParams.Length);
         for (int i = 0; i < names.Length; i++)
         { Assert.AreEqual(names[i], typeParams[i].Name); }
         var actualString = RDomCSharp.Factory.BuildSyntax(root).ToFullString();
         Assert.AreEqual(csharpCode, actualString);
      }
   }



   internal static class TypedParameterConstraintCheck
   {
      public static void Check(this ITypeParameter typeParam,
              int ordinal,
              bool hasReferenceConstraint = false,
              bool hasValueTypeConstraint = false,
              bool hasConstructorConstraint = false,
              int constraintCount = 0)
      {
         Assert.AreEqual(hasReferenceConstraint, typeParam.HasReferenceTypeConstraint);
         Assert.AreEqual(hasValueTypeConstraint, typeParam.HasValueTypeConstraint);
         Assert.AreEqual(hasConstructorConstraint, typeParam.HasConstructorConstraint);
         Assert.AreEqual(ordinal, typeParam.Ordinal);
         Assert.AreEqual(constraintCount, typeParam.ConstraintTypes.Count());
      }
   }
}
