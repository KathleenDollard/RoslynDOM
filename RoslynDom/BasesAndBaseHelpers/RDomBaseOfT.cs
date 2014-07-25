using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBase<T> : RDomBase, IDom<T>
          where T : class, IDom<T>
    {
        private ISameIntent<T> sameIntent = SameIntent_Factory.SameIntent<T>();

        protected RDomBase(IEnumerable<IPublicAnnotation> publicAnnotations)
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

        protected override bool SameIntentInternal<TLocal>(TLocal other, bool skipPublicAnnotations)
        {
            var otherAsT = other as T;
            if (otherAsT == null) return false;
            if (!CheckSameIntent(otherAsT, skipPublicAnnotations)) { return false; }
            return sameIntent.SameIntent(this as T, other as T, skipPublicAnnotations);
        }

        /// <summary>
        /// Derived classes can override this if the RoslynDom.Common implementations aren't working. 
        /// Do NOT override if the problem can be solved in the RoslynDom.Common implementations (SameIntent_xxx)
        /// </summary>
        /// <param name="other"></param>
        /// <param name="skipPublicAnnotations"></param>
        /// <returns></returns>
        protected virtual bool CheckSameIntent(T other, bool skipPublicAnnotations)
        {
            return true;
        }

    }
}
