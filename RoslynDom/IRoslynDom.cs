using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRoslynDom : IDom
    {
        ISymbol Symbol { get; }
        void RemoveFromParent();

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

