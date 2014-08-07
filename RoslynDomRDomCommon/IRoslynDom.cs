using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRoslynDom : IDom
    {
        ISymbol Symbol { get; }
        SyntaxNode TypedSyntax { get; }
        TokenWhitespaceSet TokenWhitespaceSet { get; set; }
     
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public interface IRoslynDom<T, TSymbol> : IDom<T>, IRoslynDom
       where TSymbol : ISymbol
       where T : IDom<T>
    {
        TSymbol TypedSymbol { get; }
    }

 
}

