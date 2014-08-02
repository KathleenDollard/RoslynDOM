using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal interface ICSharpBuildSyntaxWorker : IBuildSyntaxWorker
    {
        SyntaxList<AttributeListSyntax> BuildAttributeSyntax(AttributeList attributes);
        BlockSyntax GetStatementBlock(IEnumerable<IStatementCommentWhite> statements);

         TypeSyntax GetVariableTypeSyntax(IVariable itemAsVariable);
    }
}