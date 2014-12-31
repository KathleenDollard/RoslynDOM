using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;
using RoslynDom.Common;
using System.Linq;


namespace RoslynDomTests
{
   [TestClass]
   public class RegionBlockTests
   {
      private const string RegionBlockTestCategory = "RegionBlockTests";

      [TestMethod, TestCategory(RegionBlockTestCategory)]
      public void Can_retrieve_simple_block()
      {
         var csharpCode =
@"namespace ExpansionFirstTemplatesTests
{
   #region [[ _xf_ForEach(LoopOver=""Meta.Classes"", VarName=""Class"") ]]
   namespace _xf_Class_dot_Namespace
   {
      public sealed partial class _xf_Class_dot_Name
      { }
   }
#endregion
}";
         var root = RDom.CSharp.Load(csharpCode);
         var regions = root.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(1, regions.Count());
         Assert.IsNotNull(regions[0].BlockEnd);
         var root2 = root.Copy();
         var regions2 = root2.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(1, regions2.Count());
         Assert.IsNotNull(regions2[0].BlockEnd);
         Assert.AreEqual(1, regions2[0].BlockContents.NoWhitespace().Count());
      }

      [TestMethod, TestCategory(RegionBlockTestCategory)]
      public void Can_retrieve_simple_nested_blocks()
      {
         var csharpCode =
@"namespace ExpansionFirstTemplatesTests
{
   #region George
   namespace _xf_Class_dot_Namespace
   {
      public sealed partial class _xf_Class_dot_Name
      { 
         #region Fred
         public void Ron()
         { }
         #endregion
      }
    }
#endregion
}";
         var root = RDom.CSharp.Load(csharpCode);
         var regions = root.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(2, regions.Count());
         Assert.IsNotNull(regions[0].BlockEnd);
         Assert.IsNotNull(regions[1].BlockEnd);
         var root2 = root.Copy();
         var regions2 = root2.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(2, regions2.Count());
         Assert.IsNotNull(regions2[0].BlockEnd);
         Assert.IsNotNull(regions2[1].BlockEnd);
         Assert.AreEqual(1, regions2[0].BlockContents.NoWhitespace().Count());
      }

      [TestMethod]
      public void Can_retrieve_region_at_root()
      {
         var csharpCode =
@"#region George
   namespace _xf_Class_dot_Namespace
   {
      public sealed partial class _xf_Class_dot_Name
      { 
         #region Fred
         public void Ron()
         { }
         #endregion
      }
    }
#endregion
";
         var root = RDom.CSharp.Load(csharpCode);
         var regions = root.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(2, regions.Count());
         Assert.IsNotNull(regions[0].BlockEnd);
         Assert.IsNotNull(regions[1].BlockEnd);
         var root2 = root.Copy();
         var regions2 = root2.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(2, regions2.Count());
         Assert.IsNotNull(regions2[0].BlockEnd);
         Assert.IsNotNull(regions2[1].BlockEnd);
         Assert.AreEqual(1, regions2[0].BlockContents.NoWhitespace().Count());
         Assert.AreEqual(1, regions2[1].BlockContents.NoWhitespace().Count());
      }

      [TestMethod]
      public void Can_retrieve_regions_with_multiple_nesting()
      {
         var csharpCode =
@"namespace ExpansionFirstTemplatesTests
{
   #region [[ _xf_TemplateStart() ]]
   using System;
   using System.ComponentModel;

   #region [[ _xf_ForEach(LoopOver=""Meta.Classes"", VarName=""Class"") ]]
   namespace _xf_Class_dot_Namespace
   {
      public sealed partial class _xf_Class_dot_Name
      { }
   }
#endregion
#endregion
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var regions = root.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(2, regions.Count());
         Assert.IsNotNull(regions[0].BlockEnd);
         Assert.IsNotNull(regions[1].BlockEnd);
         var root2 = root.Copy();
         var regions2 = root2.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(2, regions2.Count());
         Assert.IsNotNull(regions2[0].BlockEnd);
         Assert.IsNotNull(regions2[1].BlockEnd);
         Assert.AreEqual(5, regions2[0].BlockContents.NoWhitespace().Count());
         Assert.AreEqual(1, regions2[1].BlockContents.NoWhitespace().Count());
      }

      [TestMethod]
      public void Can_retrieve_regions_with_mulipart_namespace()
      {
         var csharpCode =
@"namespace ExpansionFirstTemplatesTests.PropertyChanged
{
   using System;
   using System.ComponentModel;

   #region [[ _xf_ForEach(LoopOver=""Meta.Classes"", VarName=""Class"") ]]
   namespace _xf_Class_dot_Namespace
   {
      public sealed partial class _xf_Class_dot_Name
      { }
   }
#endregion
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var regions = root.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(1, regions.Count());
         Assert.IsNotNull(regions[0].BlockEnd);
         var root2 = root.Copy();
         var regions2 = root2.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(1, regions2.Count());
         Assert.IsNotNull(regions2[0].BlockEnd);
         Assert.AreEqual(1, regions2[0].BlockContents.NoWhitespace().Count());
      }

      //      [TestMethod]
      //      public void Can_retrieve_nested_region()
      //      {
      //         var csharpCode =
      //@"  using ExpansionFirst.Support;

      //namespace ExpansionFirstTemplateTests
      //{
      //namespace NotifyPropertyChanged
      //{
      //   #region  _xf_MakeFileForEach(Over=""Meta.Classes"", VarName=""class_"") 
      //   using System.ComponentModel;

      //   #region [[ _xf_ForEach(LoopOver=""Meta.Classes"", VarName=""Class"") ]]
      //   namespace _xf_Class_namespaceName
      //   {
      //      public sealed partial class _xf_Class_dot_Name : INotifyPropertyChanged
      //      {
      //         public event PropertyChangedEventHandler PropertyChanged;

      //         #region [[_xf_ForEach(LoopOver=""_xf_Class_dot_Properties"", LoopVar=""Property"") ]]
      //         #endregion
      //      }
      //   }

      //   #endregion

      //#endregion
      //}
      //}";
      //         var root = RDom.CSharp.Load(csharpCode);
      //         var regions = root.Descendants.OfType<IDetailBlockStart>().ToArray();
      //         Assert.AreEqual(3, regions.Count());
      //         Assert.IsNotNull(regions[0].BlockEnd);
      //         Assert.IsNotNull(regions[1].BlockEnd);
      //         Assert.IsNotNull(regions[2].BlockEnd);
      //         var root2 = root.Copy();
      //         var regions2 = root2.Descendants.OfType<IDetailBlockStart>().ToArray();
      //         Assert.AreEqual(3, regions2.Count());
      //         Assert.IsNotNull(regions2[0].BlockEnd);
      //         Assert.IsNotNull(regions2[1].BlockEnd);
      //         Assert.IsNotNull(regions2[2].BlockEnd);
      //         Assert.AreEqual(5, regions2[0].BlockContents.NoWhitespace().Count());
      //         Assert.AreEqual(1, regions2[1].BlockContents.NoWhitespace().Count());
      //         Assert.AreEqual(4, regions2[2].BlockContents.NoWhitespace().Count());
      //         Assert.AreEqual(2, regions2[3].BlockContents.NoWhitespace().Count());
      //      }
   }
}

//#region George
//namespace _xf_Class_dot_Namespace
//{
//   public sealed partial class _xf_Class_dot_Name
//   {
//      #region Fred
//      public void Ron()
//      { }
//      #endregion
//   }
//}
//#endregion