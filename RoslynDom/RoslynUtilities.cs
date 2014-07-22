using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class RoslynUtilities
    {
        public static SyntaxNode Format(SyntaxNode node)
        {
           // var formattingOptions = FormattingOptions.;
            var span = node.FullSpan;
            node = Formatter.Format(node, span, new CustomWorkspace());
            return node;
        }

 
        //internal static bool TryAddSyntaxNode<TInput, TSyntaxNode, TRDom>(IList<TSyntaxNode> list, TInput member, Func<TRDom, TSyntaxNode> makeDelegate)
        //         where TRDom : class
        //{
        //    var memberAsT = member as TRDom;
        //    if (memberAsT == null) return false;
        //    var newItem =makeDelegate(memberAsT);
        //    if (newItem == null) throw new InvalidOperationException();
        //    list.Add(newItem);
        //    return true; ;

        //}

        //internal static TReturn UpdateNodeIfListNotEmpty<T, TReturn>(SyntaxList<T> list, TReturn input, Func<TReturn, SyntaxList<T>, TReturn> makeDelegate)
        //    where T : SyntaxNode
        //    where TReturn : SyntaxNode
        //{
        //    if (list.Any()) { return makeDelegate(input, list); }
        //    return input;

        //}

        //internal static TReturn UpdateNodeIfItemNotNull<T, TReturn>(T item, TReturn input, Func<TReturn, T, TReturn> makeDelegate)
        //        where T : SyntaxNode
        //        where TReturn : SyntaxNode
        //{
        //    if (item != null) { return makeDelegate(input, item); }
        //    return input;

        //}

 
        public static AccessModifier GetAccessibilityFromSymbol(ISymbol symbol)
        {
            if (symbol == null) { return AccessModifier.NotApplicable; }
            return (AccessModifier)symbol.DeclaredAccessibility;
        }
    }
}
