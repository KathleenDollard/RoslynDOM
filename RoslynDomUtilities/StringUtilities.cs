using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.Common
{
    public static class StringUtilities
    {
      public static string NameFromQualifiedName(string qName)
      {
         if (qName.Contains(".")) return qName.SubstringAfterLast(".");
         return qName;
      }

      public static string NamespaceFromQualifiedName(string qName)
      {
         if (qName.Contains(".")) return qName.SubstringBeforeLast(".");
         return qName;
      }

      public static string CamelCase(string name)
      {
         if (name.StartsWith("_")) name = name.Substring(1);
         name = name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
         return "_" + name;
      }

      public static string ReplaceFirst(this string input, string replace, string replaceWith)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (string.IsNullOrEmpty(replace)) throw new InvalidOperationException("Replace string cannot be null or an empty string");
            if (replaceWith==null) throw new InvalidOperationException("Replace with string cannot be null. To remove first occurrence, use an empty string");
            if (input.Contains(replace))
            {
                var start = input.SubstringBefore(replace);
                var end = input.SubstringAfter(replace);
                return start + replaceWith + end;
            }
            return input;
        }

        public static string SubstringAfter(this string input, string delimiter)
        {
            if (input == null) { return null; }
            if (delimiter == null) { throw new InvalidOperationException(); }
            var pos = input.IndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(pos + delimiter.Length );
        }

        public static string SubstringAfterLast(this string input, string delimiter)
        {
            if (input == null) { return null; }
            if (delimiter == null) { throw new InvalidOperationException(); }
            var pos = input.LastIndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(pos + delimiter.Length);
        }

        public static string SubstringBefore(this string input, string delimiter)
        {
            if (input == null) { return null; }
            if (delimiter == null) { throw new InvalidOperationException(); }
            var pos = input.IndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(0, pos);
        }

        public static string SubstringBeforeLast(this string input, string delimiter)
        {
            if (input == null) { return null; }
            if (delimiter == null) { throw new InvalidOperationException(); }
            var pos = input.LastIndexOf(delimiter, StringComparison.Ordinal);
            if (pos < 0) return "";
            return input.Substring(0, pos);
        }

        public static string JoinString(this IEnumerable<string> stringList)
        {
            return string.Join("", stringList);
        }
    }
}
