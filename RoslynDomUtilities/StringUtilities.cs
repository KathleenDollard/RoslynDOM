using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
         return "";
      }

      public static string CamelCase(string name)
      {
         var prefix = "";
         if (name.StartsWith("_"))
         {
            name = name.Substring(1);
            prefix = "_";
         }
         name = name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
         return prefix + name;
      }

      public static string PascalCase(string name)
      {
         var prefix = "";
         if (name.StartsWith("_"))
         {
            name = name.Substring(1);
            prefix = "_";
         }
         name = name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
         return prefix + name;
      }

      public static string ReplaceFirst(this string input, string replace, string replaceWith)
      {
         if (string.IsNullOrEmpty(input)) return input;
         if (string.IsNullOrEmpty(replace)) throw new InvalidOperationException("Replace string cannot be null or an empty string");
         if (replaceWith == null) throw new InvalidOperationException("Replace with string cannot be null. To remove first occurrence, use an empty string");
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
         return input.Substring(pos + delimiter.Length);
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

      private static Regex _allWhitespace;

      private static Regex allWhitespace
      {
         get
         {
            if (_allWhitespace == null) _allWhitespace = new Regex(@"\s+", RegexOptions.Compiled);
            return _allWhitespace;
         }
      }

      /// <summary>
      /// This method is expected to be used in testing to avoid file header comments breaking diffs
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public static string RemoveFileHeaderComments(string input)
      {
         var lines = input.Split(new[] { "\r\n" }, StringSplitOptions.None);
         var i = 0;
         while (i < lines.Count())
         {
            if (!(lines[i].TrimStart().StartsWith("//") || string.IsNullOrWhiteSpace(lines[i]))) { break; }
            i++;
         }
         return string.Join("\r\n", lines.Skip(i));
      }

      public static string NormalizeWhitespace(this string input, string replaceWith = " ")
      {
         return allWhitespace.Replace(input, replaceWith);
      }
   }
}