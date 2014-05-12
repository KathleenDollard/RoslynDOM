using KadGen.Common.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class TestRoslynSyntaxElementInterface : TestRoslynSyntaxElementWithAttributes<InterfaceDeclarationSyntax>
    {
        public TestRoslynSyntaxElementInterface() : base(typeof(RoslynSyntaxSupportInterface), "Interface")
        {
            totalFlattenedCountRoot = 5;
            totalNotFlattenedCountRoot = 2;
            totalFlattenedCountNamespaceB = 3;
            totalNotFlattenedCountNamespaceB = 2;
        }
     }
}