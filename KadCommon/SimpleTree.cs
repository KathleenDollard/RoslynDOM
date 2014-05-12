using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KadGen.Common
{

    public class SimpleTreeNode<T>
    {
        private T _value;
        private List<SimpleTreeNode<T>> _children;

        public SimpleTreeNode(T value, IEnumerable<SimpleTreeNode<T>> children)
        {
            _value = value;
            _children = new List<SimpleTreeNode<T>>(children);
        }

        private SimpleTreeNode(T value)
        {
            _value = value;
            _children = new List<SimpleTreeNode<T>>();
        }

        private void AddChild(SimpleTreeNode<T> newItem)
        {
            _children.Add(newItem);
        }

        public string Report(string indent)
        {
            var sb = new StringBuilder();
            sb.AppendLine(indent + (this.HasValue ? this.Value.ToString() : ""));
            var i = 0;
            foreach (var child in this.Children)
            {
                sb.Append(child.Report(indent + "   "));
                i++;
            }
            return sb.ToString();
        }


        public T Value { get { return _value; } }
        public IEnumerable<SimpleTreeNode<T>> Children { get { return _children; } }

        public bool HasValue
        { get { return (!object.Equals(this.Value, default(T))); } }

        public static SimpleTreeNode<T> CreateTreeFromList(IEnumerable<T> list,
                    Func<T, T, T, bool> isContainedPredicate)
        {
            var current = 0;
            var parent = new SimpleTreeNode<T>(default(T));
            //var containedList = CreateTreeListFromList(list, parent, ref current, isContainedPredicate);
            var handled = CreateTreeListFromList(list, parent, isContainedPredicate);
            return parent;
        }

        private static int CreateTreeListFromList(
                IEnumerable<T> list, SimpleTreeNode<T> parent,
             Func<T, T, T, bool> isContainedPredicate)
        {
            var innerList = new List<SimpleTreeNode<T>>();
            var array = list.ToArray();
            var count = list.Count();
            var i = 0;
            var handled = 0;

            while (i < list.Count())
            {
                T thisOne = array[i];
                T next = (i >= count - 1) ? default(T) : array[i + 1];
                var isContained = (parent.Value == null ? true : isContainedPredicate(thisOne, parent.Value, next));
                if (isContained)
                {
                    var tail = list.Skip(i + 1);
                    handled++;
                    var newChild = new SimpleTreeNode<T>(thisOne);
                    handled += CreateTreeListFromList(tail, newChild, isContainedPredicate);
                    i += handled - i;
                    parent.AddChild(newChild);
                }
                else { break; }
            }
            return handled;
        }

    }
}
