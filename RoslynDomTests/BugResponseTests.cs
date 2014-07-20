using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass ]
    public class BugResponseTests
    {
        private const string BugResponseCategory = "BugResponse";

        [TestMethod, TestCategory(BugResponseCategory)]
        public void Public_root_annotations_failed_on_expansion_first()
        {
            var csharpCode = @"
             //[[file:_xf_OutputForEach(LoopOver=""SemanticLogs"", LoopVarName=""_logClass_"")] 
                    
            public class MyClass
            { }
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            // The issue here is tha the system is unforgiving of the single close square bracket  
            Assert.IsFalse (root.PublicAnnotations.HasPublicAnnotation("_xf_OutputForEach"));
        }

    }
}
