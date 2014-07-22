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

        public RDomList(IDom parent)
        { Parent = parent; }

        public IDom Parent { get; private set; }
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
            UpdateParent(item);
            _list.Add(item);
        }

        public void AddOrMoveRange(IEnumerable<T> items)
        {
            // Don't use AddRange because we need to manage parents
           foreach (var item in items)
            { AddOrMove(item); }
        }

        public void InsertOrMove(int index, T item )
        {
            UpdateParent(item);
            if (index >= _list.Count() - 1)
            { _list.Add(item); }
            else
            { _list.Insert(index, item); }
        }

        public void InsertOrMoveAfter(T existing,T itemToInsert )
        {
            var pos = _list.IndexOf(existing);
            InsertOrMove(pos + 1, itemToInsert);
        }

        public void InsertOrMoveBefore(T existing, T itemToInsert)
        {
            var pos = _list.IndexOf(existing);
            InsertOrMove(pos, itemToInsert);
        }

        public void Remove(T item)
        {
            SetParent(item, null);
            _list.Remove(item);
        }

        public void Clear()
        {
            var items = _list.ToList();
            foreach (var item in items)
            { _list.Remove(item); }
        }

        private void UpdateParent(T item)
        {
            // TODO: Remove item from the other list
            if (item.Parent != null && item.Parent != this.Parent && item.Parent.Parent != this.Parent) { throw new NotImplementedException(); }

            // Since it's important that people don't change the parent except here, I'm using reflection 
            SetParent(item, this.Parent);
        }

        private void SetParent(T item, IDom parent)
        {
            ReflectionUtilities.SetPropertyValue(item, "Parent", parent);
        }
    }
}
