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

        public static string GetOuterName(IDom item)
        {
            var name = "";
            var itemHasName = item as IHasName;
            if (itemHasName != null) name += itemHasName.Name;
            var parent = item.Parent;
            do
            {
                if (parent is IRoot) // at top of what we care about
                { break; }
                var parentHasName = parent as IHasName;
                if (parentHasName != null)
                {
                    var delimiter = ".";
                    if (item is IType && parent is IType) delimiter = "+";
                    name = parentHasName.Name + (string.IsNullOrEmpty(name) ? "" : delimiter + name);
                }
                item = parent;
                parent = parent.Parent;
            } while (parent != null);
            return name;
        }

        public static string GetQualifiedName(IDom item)
        {
            var name = "";
            var itemHasName = item as IHasName;
            if (itemHasName != null) name += itemHasName.Name;
            var parent = item.Parent;
            do
            {
                if (parent is IRoot) // at top of what we care about
                { break; }
                var parentHasName = parent as IHasName;
                if (parentHasName != null)
                {
                    var delimiter = ".";
                    name = parentHasName.Name + (string.IsNullOrEmpty(name) ? "" : delimiter + name);
                }
                item = parent;
                parent = parent.Parent;
            } while (parent != null);
            return name;
        }
    }
}
