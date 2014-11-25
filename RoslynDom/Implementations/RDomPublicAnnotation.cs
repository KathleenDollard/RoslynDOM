using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoslynDom.Common
{
   public class RDomPublicAnnotation : RDomDetail<IPublicAnnotation>, IPublicAnnotation
   {
      private List<KeyValuePair<string, object>> _items = new List<KeyValuePair<string, object>>();

      public RDomPublicAnnotation(IDom parent, SyntaxTrivia trivia, string name)
         :base(parent,StemMemberKind.PublicAnnotation, MemberKind.PublicAnnotation, trivia )
      {
         _name = name;
      }

      internal RDomPublicAnnotation(RDomPublicAnnotation oldRDom)
          : base(oldRDom)
      {
         _name = oldRDom.Name;
         _target = oldRDom.Target;
         foreach (var key in oldRDom.Keys )
         {
            // The following line only works for types that don't need to be cloned, I think that is all we can hold in a public annotation right now. 
            AddItem(key, oldRDom.GetValue(key));
         }
      }

      private string _target;
      public string Target
      {
         get { return _target; }
         set { SetProperty(ref _target, value); }
      }

      private string _name;
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public void AddItem(string key, object item)
      { _items.Add(new KeyValuePair<string, object>(key, item)); }

          public IEnumerable<string> Keys
      {
         get { return _items.Select(x => x.Key); }
      }

      public T GetValue<T>(string key)
      {
         return (T)GetValue(key); ;
      }

      public object GetValue(string key)
      {
         var item = _items.Where(x => x.Key == key).FirstOrDefault();
         return item.Value;
      }

      public bool TryGetValue<T>(string key, out T value)
      {
         value = default(T);
         if (!HasValue(key)) { return false; }
         value = GetValue<T>(key);
         return true;
      }

      public bool HasValue(string key)
      {
         return _items.Any(x => x.Key == key);
      }

      //[ExcludeFromCodeCoverage]
      //public override object RequestValue(string propertyName)
      //{ return GetValue(propertyName); }

      //protected override bool SameIntentInternal<TLocal>(TLocal other)
      //{
      //   var otherAnnotation = other as IPublicAnnotation;
      //   foreach (var item in items)
      //   {
      //      var otherValue = otherAnnotation.GetValue(item.Key);
      //      if (otherValue == null) return false;
      //      var itemHasSameIntent = item.Value as IHasSameIntentMethod;
      //      if (itemHasSameIntent != null)
      //      { if (!itemHasSameIntent.SameIntent(otherValue)) { return false; } }
      //      if (!otherValue.Equals(item.Value)) return false;
      //   }
      //   return true;
      //}

      //public IPublicAnnotation Copy()
      //{
      //   throw new NotImplementedException();
      //}

      //[ExcludeFromCodeCoverage]
      //public override object OriginalRawItem
      //{ get { return null; } }

      //[ExcludeFromCodeCoverage]
      //public override object RawItem
      //{ get { return null; } }

    }
}
