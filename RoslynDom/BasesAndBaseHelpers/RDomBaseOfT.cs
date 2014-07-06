using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBase<T> : RDomBase, IDom<T>
          where T : class, IDom<T>
    {
        private ISameIntent<T> sameIntent = SameIntent_Factory.SameIntent<T>();

        protected RDomBase(params PublicAnnotation[] publicAnnotations)
           : base(publicAnnotations)
        { }

        protected RDomBase(T oldRDom)
             : base(oldRDom)
        { }

    
        public virtual T Copy()
        {
            var type = this.GetType();
            var constructor = type.GetTypeInfo()
                .DeclaredConstructors
                .Where(x => x.GetParameters().Count() == 1
                && typeof(T).IsAssignableFrom(x.GetParameters().First().ParameterType))
                .FirstOrDefault();
            if (constructor == null) throw new InvalidOperationException("Missing constructor for clone");
            var newItem = constructor.Invoke(new object[] { this });
            return (T)newItem;
        }

        internal override bool SameIntentInternal<TLocal>(TLocal other, bool includePublicAnnotations)
        {
            return sameIntent.SameIntent(this as T, other as T, includePublicAnnotations);
            //if (other == null) { return false; }
            //if (!typeof(T).IsAssignableFrom(typeof(TLocal))) { return false; }
            //var otherAsT = other as T;
            //if (!CheckSameIntent(otherAsT, includePublicAnnotations)) { return false; }
            //return true;
        }

        /// <summary>
        /// Derived classes should override this to determine intent
        /// </summary>
        /// <param name="other"></param>
        /// <param name="includePublicAnnotations"></param>
        /// <returns></returns>
        protected virtual bool CheckSameIntent(T other, bool includePublicAnnotations)
        {
            var otherItem = other as RDomBase;
            if (!PublicAnnotations.SameIntent(other.PublicAnnotations, includePublicAnnotations)) { return false; }
            return true;
        }

         protected bool CheckSameIntentChildList<TChild>(IEnumerable<TChild> thisList,
           IEnumerable<TChild> otherList)
              where TChild : class, IDom<TChild>
        {
            if (thisList == null) return (otherList == null);
            if (otherList == null) return false;
            if (thisList.Count() != otherList.Count()) return false;
            if (thisList == null) return false; // can't happen, suppresse FxCop error
            foreach (var item in thisList)
            {
                var otherItem = otherList.Where(x => item.Matches(x)).FirstOrDefault();
                if (otherItem == null) return false;
                if (!item.SameIntent(otherItem)) return false;
            }
            return true;
        }

    }
}
