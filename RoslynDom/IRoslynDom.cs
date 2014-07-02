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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public interface IRoslynDom<T, TSyntax, TSymbol> : IDom<T>
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
        where T : IDom<T>
    {
        TSymbol TypedSymbol { get; }
        TSyntax TypedSyntax { get; }
    }
}

