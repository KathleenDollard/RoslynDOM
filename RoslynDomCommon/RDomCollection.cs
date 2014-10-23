using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public class RDomCollection<T> : IEnumerable<T>
       where T : class, IDom
   {
      private List<T> _list = new List<T>();

      public RDomCollection(IDom parent)
      { Parent = parent; }

      public IDom Parent { get; private set; }
      public IEnumerator<T> GetEnumerator()
      {
         return _list.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _list.GetEnumerator();
      }

      public bool AddOrMove(T item)
      {
         UpdateParent(item);
         _list.Add(item);
         return true;
      }

      public bool AddOrMove<TLocal>(TLocal item)
      {
         var itemAsKind = item as T;
         if (itemAsKind == null) return false;
         return AddOrMove(itemAsKind);
      }

      public bool AddOrMoveRange(IEnumerable<T> items)
      {
         // Don't use AddRange because we need to manage parents
         if (items == null) return false;
         foreach (var item in items)
         { AddOrMove(item); }
         return true;
      }

      public bool AddOrMoveRange<TLocal>(IEnumerable<TLocal> items)
      {
         var itemsAsKind = items.OfType<T>();
         if (items.Count() != items.Count()) return false;
         return AddOrMoveRange(itemsAsKind);
      }

      public bool InsertOrMove(int index, T item)
      {
         UpdateParent(item);
         if (index >= _list.Count() - 1)
         { _list.Add(item); }
         else
         { _list.Insert(index, item); }
         return true;
      }

      public bool InsertOrMove<TLocal>(int index, TLocal item)
      {
         var itemAsKind = item as T;
         if (itemAsKind == null) return false;
         return InsertOrMove(index, itemAsKind);
      }

      public bool InsertOrMoveAfter(T existing, T itemToInsert)
      {
         var pos = _list.IndexOf(existing);
         InsertOrMove(pos + 1, itemToInsert);
         return true;
      }

      public bool InsertOrMoveAfter<TLocal>(TLocal existing, TLocal itemToInsert)
      {
         var existingAsKind = existing as T;
         var itemToInsertAsKind = itemToInsert as T;
         if (existingAsKind == null || itemToInsertAsKind == null) return false;
         return InsertOrMoveAfter(existingAsKind, itemToInsertAsKind);
      }

      public bool InsertOrMoveBefore(T existing, T itemToInsert)
      {
         var pos = _list.IndexOf(existing);
         InsertOrMove(pos, itemToInsert);
         return true;
      }

      public bool InsertOrMoveBefore<TLocal>(TLocal existing, TLocal itemToInsert)
      {
         var existingAsKind = existing as T;
         var itemToInsertAsKind = itemToInsert as T;
         if (existingAsKind == null || itemToInsertAsKind == null) return false;
         return InsertOrMoveBefore(existingAsKind, itemToInsertAsKind);
      }

      public bool Remove(T item)
      {
         SetParent(item, null);
         _list.Remove(item);
         return true;
      }

      public bool Remove<TLocal>(TLocal item)
      {
         var itemAsKind = item as T;
         if (itemAsKind == null) return false;
         return Remove( itemAsKind);
      }

      public bool Replace(T oldItem, T newItem)
      {
         var pos = _list.IndexOf(oldItem);
         Remove(oldItem);
         InsertOrMove(pos, newItem);
         return true;
      }

      public bool Replace<TLocal>(TLocal oldItem, TLocal newItem)
      {
         var oldItemAsKind = newItem as T;
         var newItemAsKind = newItem as T;
         if (oldItemAsKind == null || newItemAsKind == null) return false;
         return Replace(oldItemAsKind, newItemAsKind);
      }

      public RDomCollection <T> Copy(IDom newParent)
      {
         var newList = new RDomCollection<T>(newParent);
         foreach (var item in _list)
         {
            var copyMethod = item.GetType().GetMethod("Copy");
            if (copyMethod == null) throw new NotImplementedException();
            var newItem = copyMethod.Invoke(item, null);
            newList.AddOrMove(newItem);
         }
         return newList;
      }

      public void Clear()
      {
         var items = _list.ToList();
         foreach (var item in items)
         { _list.Remove(item); }
      }

      private void UpdateParent(T item)
      {
         if (item == null) return; // this happens for accessors which use RDomList to manage parent
                                   // TODO: Remove item from the other list
         if (item.Parent != null && item.Parent != this.Parent && item.Parent.Parent != this.Parent) { throw new NotImplementedException(); }

         // Since it's important that people don't change the parent except here, I'm using reflection 
         SetParent(item, this.Parent);
      }

      private static void SetParent(T item, IDom parent)
      {
         if (item == null) return; // this happens for accessors which use RDomList to manage parent
         ReflectionUtilities.SetPropertyValue(item, "Parent", parent);
      }
   }
}
