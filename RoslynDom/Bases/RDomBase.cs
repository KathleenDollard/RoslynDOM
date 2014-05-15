using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary>
    /// Base class for Roslyn Dom navigation tree
    /// </summary>
    public abstract class RDomBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Return type is object, not SyntaxNode to match interface
        /// </remarks>
        public abstract object RawItem { get; }

        public abstract string Name { get; }

        public abstract string QualifiedName { get; }

        public abstract ISymbol Symbol { get; }
    }

    public abstract class RDomBase<T> : RDomBase
        where T : SyntaxNode
    {
        private T _rawItem;
        private ISymbol _symbol;

        protected RDomBase(T rawItem)
        {
            _rawItem = rawItem;
        }

        internal T TypedRawItem
        {
            get
            { return _rawItem; }
        }

        public override object RawItem
        {
            get
            { return _rawItem; }
        }

        public override ISymbol Symbol
        {
            get
            {
                if (_symbol == null)
                { _symbol = GetSymbol(); }
                return _symbol;
            }
        }

        private SemanticModel GetModel()
        {
            var tree = TypedRawItem.SyntaxTree;
            var compilation = CSharpCompilation.Create("MyCompilation",
                                           options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                           syntaxTrees: new[] { tree },
                                           references: new[] { Mscorlib });
            var model = compilation.GetSemanticModel(tree);
            return model;

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


        private ISymbol GetSymbol()
        {
            var model = GetModel();
            return model.GetDeclaredSymbol(TypedRawItem);
        }
    }

    public abstract class RDomSyntaxNodeBase<T> : RDomBase<T>
         where T : SyntaxNode
    {
        // TODO: Consider why this isn't collapsed into the RDomBase<T>
        private T _rawItem;

        protected RDomSyntaxNodeBase(T rawItem) : base(rawItem)
        {
            _rawItem = rawItem;
        }

    }
}
