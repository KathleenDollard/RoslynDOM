using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynK
{
    /// <summary>
    /// Base class for Roslyn K navigation tree
    /// </summary>
    public abstract class KBase
    {
        public abstract string Name { get; }
        public abstract string QualifiedName { get; }
        public abstract string BestInContextName { get; }
    }

    public abstract class KBase<T> : KBase
    {
        private T _rawItem;

        protected KBase(T rawItem)
        {
            _rawItem = rawItem;
        }

        internal T TypedRawItem
        {
            get
            { return _rawItem; }
        }

        public object RawItem
        {
            get
            { return _rawItem; }
        }
    }

    public abstract class KSyntaxNodeBase<T> : KBase<T>
    {
        private SyntaxTree _syntaxTree;
        private SemanticModel _model;
        private SymbolInfo _symbolInfo;

        protected KSyntaxNodeBase(T rawItem) : base(rawItem)
        {
            var syntax = rawItem as SyntaxNode;
            _syntaxTree = syntax.SyntaxTree;
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { _syntaxTree }, references: new[] { Mscorlib });
            _model = compilation.GetSemanticModel(_syntaxTree);
            _symbolInfo = _model.GetSymbolInfo(syntax);
        }

        private MetadataReference mscorlib;
        private MetadataReference Mscorlib
        {
            get
            {
                if (mscorlib == null)
                {
                    mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
                }

                return mscorlib;
            }
        }


        protected ISymbol Symbol
        {
            get
            {
                var symbol = _symbolInfo.Symbol;
                if (symbol == null)
                { Console.WriteLine(); }
                return _symbolInfo.Symbol; }
        }
    }
}
