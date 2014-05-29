using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRoslynDom : IDom
    {
        ISymbol Symbol { get; }
    }

    public interface IRoslynDom<TSyntax, TSymbol> : IDom
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
    {
        TSymbol TypedSymbol { get; }
        TSyntax TypedSyntax { get; }
    }
}

