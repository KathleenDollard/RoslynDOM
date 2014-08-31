using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class RoslynUtilities
    {
    
        public static AccessModifier GetAccessibilityFromSymbol(ISymbol symbol)
        {
            if (symbol == null) { return AccessModifier.None; }
            return (AccessModifier)symbol.DeclaredAccessibility;
        }

        public static string GetOuterName(IDom item)
        {
            var name = "";
            var itemToName = item;
            var itemHasName = itemToName as IHasName;
            if (itemHasName != null) name += itemHasName.Name;
            if (!(itemToName is IStemMember || itemToName is ITypeMember)) return name;
            var parent = itemToName.Parent;
            do
            {
                // null is legal here because objects may be unattached
                if (parent == null || parent is IRoot ) // at top of what we care about
                { break; }
                var parentHasName = parent as IHasName;
                if (parentHasName != null)
                {
                    var delimiter = ".";
                    if (itemToName is IType && parent is IType) delimiter = "+";
                    name = parentHasName.Name + (string.IsNullOrEmpty(name) ? "" : delimiter + name);
                }
                itemToName = parent;
                parent = parent.Parent;
            } while (parent != null);
            return name;
        }

        public static string GetQualifiedName(IDom item)
        {
            if (item == null) throw new NotImplementedException();
            var name = "";
            var itemHasName = item as IHasName;
            if (itemHasName != null) name += itemHasName.Name;
            var parent = item.Parent;
            do
            {
                // null is legal here because objects may be unattached
                if (parent == null || parent is IRoot) // at top of what we care about
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
