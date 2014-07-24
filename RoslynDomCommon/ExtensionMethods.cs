using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> PreviousSiblings<T>(this IEnumerable<T> list, T item)
        {
            var ret = new List<T>();
            if (!list.Contains(item)) return ret;
            foreach (var member in list)
            {
                if (member.Equals(item)) break;
                ret.Add(member);
            }
            return ret;
        }

        public static IEnumerable<T> FollowingSiblings<T>(this IEnumerable<T> list, T item)
        {
            var ret = new List<T>();
            if (!list.Contains(item)) return ret;
            var startTaking = false;
            foreach (var member in list)
            {
                if (startTaking) ret.Add(member);
                if (member.Equals(item)) startTaking = true;
            }
            return ret;
        }

        public static IEnumerable<T> PreviousSiblingsUntil<T>(this IEnumerable<T> list, T item, Func<T, bool> condition)
        {
            var ret = new List<T>();
            if (!list.Contains(item)) return ret;
            var reversed = new List<T>(list);
            var startTaking = false;
            reversed.Reverse();
            foreach (var member in reversed)
            {
                if (startTaking)
                {
                    if (condition(member)) break;
                    ret.Add(member);
                }
                if (member.Equals(item)) startTaking = true;
            }
            ret.Reverse();
            return ret;
        }

        public static IEnumerable<T> FollowingSiblingsUntil<T>(this IEnumerable<T> list, T item, Func<T, bool> condition)
        {
            var ret = new List<T>();
            if (!list.Contains(item)) return ret;
            var startTaking = false;
            foreach (var member in list)
            {
                if (startTaking)
                {
                    if (condition(member)) break;
                    ret.Add(member);
                }
                if (member.Equals(item)) startTaking = true;
            }
            return ret;
        }

        public static bool IsInteger(object value)
          {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int;
        }

        public static bool IsFloatingPint(object value)
        {
            return  value is float
                    || value is double;
        }
    }
}
