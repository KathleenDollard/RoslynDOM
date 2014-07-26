using System.Linq;
using Microsoft.CodeAnalysis;
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            // The issue here is tha the system is unforgiving of the single close square bracket  
            Assert.IsFalse (root.PublicAnnotations.HasPublicAnnotation("_xf_OutputForEach"));
        }

        [TestMethod, TestCategory(BugResponseCategory)] // #46
        public void Can_parse_for_property_bug_46()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                            protected int _flotDialogue
                            {
                                get { return MyStaticClass1._flotDialogue; }
                                set { MyStaticClass1._flotDialogue = value; }
                            }
                        }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomProperty)root.Classes.First().Properties.First()).TypedSymbol as IPropertySymbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("_flotDialogue", symbol.Name);
        }

    }
}
