using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class RoslynUtilities
    {

        private static MetadataReference mscorlib;
        public static MetadataReference Mscorlib
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


        public static bool TryAddSyntaxNode<TInput, TSyntaxNode, TRDom>(IList<TSyntaxNode> list, TInput member, Func<TRDom, TSyntaxNode> makeDelegate)
                 where TRDom : class
        {
            var memberAsT = member as TRDom;
            if (memberAsT == null) return false;
            var newItem =makeDelegate(memberAsT);
            if (newItem == null) throw new InvalidOperationException();
            list.Add(newItem);
            return true; ;

        }

        public static TReturn UpdateNodeIfListNotEmpty<T, TReturn>(SyntaxList<T> list, TReturn input, Func<TReturn, SyntaxList<T>, TReturn> makeDelegate)
            where T : SyntaxNode
            where TReturn : SyntaxNode
        {
            if (list.Any()) { return makeDelegate(input, list); }
            return input;

        }

        public static TReturn UpdateNodeIfItemNotNull<T, TReturn>(T item, TReturn input, Func<TReturn, T, TReturn> makeDelegate)
                where T : SyntaxNode
                where TReturn : SyntaxNode
        {
            if (item != null) { return makeDelegate(input, item); }
            return input;

        }

          public static AccessModifier GetAccessibilityFromSymbol(ISymbol symbol)
        {
            if (symbol == null) { return AccessModifier.NotApplicable; }
            return (AccessModifier)symbol.DeclaredAccessibility;
        }
    }
}
