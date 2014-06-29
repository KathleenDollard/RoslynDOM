using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public static class StringUtilities
    {

        public static string SubstringAfter(this string input, string delimiter)
        {
            if (input == null) { return null; }
            var pos = input.IndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(pos + delimiter.Length );
        }

        public static string SubstringAfterLast(this string input, string delimiter)
        {
            if (input == null) { return null; }
            var pos = input.LastIndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(pos + delimiter.Length);
        }

        public static string SubstringBefore(this string input, string delimiter)
        {
            if (input == null) { return null; }
            var pos = input.IndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(0, pos);
        }

        public static string SubstringBeforeLast(this string input, string delimiter)
        {
            if (input == null) { return null; }
            var pos = input.LastIndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(0, pos);
        }
    }
}
