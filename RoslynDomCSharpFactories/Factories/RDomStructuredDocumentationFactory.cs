using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class StructuredDocumentationFactory : IStructuredDocumentationFactory
    {
        public FactoryPriority Priority
        { get { return FactoryPriority.Normal; } }

        public bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            // Always tries
            return true;
        }

        public IEnumerable<IStructuredDocumentation> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return new List<IStructuredDocumentation>() { new RDomStructuredDocumentation() };
        }

        public IEnumerable<SyntaxNode> BuildSyntax(IStructuredDocumentation item)
        {
            return null;
        }

    }
}
