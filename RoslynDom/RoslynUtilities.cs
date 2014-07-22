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

        public static AccessModifier GetAccessibilityFromSymbol(ISymbol symbol)
        {
            if (symbol == null) { return AccessModifier.NotApplicable; }
            return (AccessModifier)symbol.DeclaredAccessibility;
        }
    }
}
