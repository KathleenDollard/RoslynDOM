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

        protected RDomBase(IEnumerable<PublicAnnotation> publicAnnotations)
          : base(publicAnnotations)
        { }

        //protected RDomBase(params PublicAnnotation[] publicAnnotations)
        //   : base(publicAnnotations)
        //{ }

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

        protected override bool SameIntentInternal<TLocal>(TLocal other, bool includePublicAnnotations)
        {
            if (!CheckSameIntent(other as T, includePublicAnnotations)) { return false; }
            return sameIntent.SameIntent(this as T, other as T, includePublicAnnotations);
        }

        /// <summary>
        /// Derived classes can override this if the RoslynDom.Common implementations aren't working
        /// </summary>
        /// <param name="other"></param>
        /// <param name="includePublicAnnotations"></param>
        /// <returns></returns>
        protected virtual bool CheckSameIntent(T other, bool includePublicAnnotations)
        {
            return true;
        }

    }
}
