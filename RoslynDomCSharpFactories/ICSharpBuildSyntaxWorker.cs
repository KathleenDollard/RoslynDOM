using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal interface ICSharpBuildSyntaxWorker : IBuildSyntaxWorker
    {
        SyntaxList<AttributeListSyntax> BuildAttributeSyntax(AttributeCollection attributes);
        BlockSyntax GetStatementBlock(IEnumerable<IStatementAndDetail> statements);

         TypeSyntax GetVariableTypeSyntax(bool isImplicitlyTypes, IReferencedType type);
    }
}