using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDoStatementFactory
         : RDomStatementFactory<RDomDoStatement, DoStatementSyntax>
    {
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as DoStatementSyntax;
            var newItem = new RDomDoStatement(syntaxNode, parent, model);
            newItem.TestAtEnd = true; 
            return LoopFactoryHelper.CreateFrom<IDoStatement>(newItem, syntax.Condition, syntax.Statement, parent, model);
        }

          public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IDoStatement ;
            return LoopFactoryHelper.BuildSyntax<IDoStatement>(itemAsT, (c, s) => SyntaxFactory.DoStatement(s, c));
        }

 
    }
}
