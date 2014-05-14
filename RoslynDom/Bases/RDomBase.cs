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
        public abstract string Name { get; }
        public abstract string QualifiedName { get; }
        public abstract string BestInContextName { get; }
    }

    public abstract class RDomBase<T> : RDomBase
    {
        private T _rawItem;

        protected RDomBase(T rawItem)
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

    public abstract class RDomSyntaxNodeBase<T> : RDomBase<T>
    {
        private T _rawItem;

        protected RDomSyntaxNodeBase(T rawItem) : base(rawItem)
        {
            _rawItem = rawItem;
        }

    }
}
