using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class RDomList<T> : IEnumerable<T>
        where T : IDom
    {
        private List<T> _list = new List<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _list .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void AddOrMove(T item)
        {

        }

        public void InsertOrMove(T item, int index)
        {

        }

        public void InsertOrMoveAfter(T item, T existing)
        {

        }

        public void InsertOrMoveBefore(T item, T existing)
        {

        }

        public void Remove(T item)
        {

        }
    }
}
