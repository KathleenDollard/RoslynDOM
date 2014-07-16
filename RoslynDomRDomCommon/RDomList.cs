using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class ParentedItemList<T> : IEnumerable<T>
        where T : IDom
    {
        private List<T> list;
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        //public void AddOrMoveItem(T item, IDom parent)
        //{
        //    if (item.Parent != null)
        //    {
        //        var removeMethodInfo = ReflectionUtilities.FindMethod(item.GetType(), "RemoveChild", new Type[] { typeof(IDom) });
        //        item.Parent.RemoveChild(item);
        //    }
        //    list.Add(item);
        //    var methodInfo = ReflectionUtilities.FindMethod(item.GetType(), "SetParent", new Type[] { typeof(IDom) });
        //    if (methodInfo == null) throw new InvalidOperationException();
        //}

        //public void RemoveMember(T item)
        //{
        //    item.Parent = null;
        //    list.Remove(item);
        //}
    }
}
