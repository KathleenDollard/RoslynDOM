using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class Guardian
    {
        private static Guardian _inform;

        private Guardian() { }

        public static Guardian Assert
        {
            get
            {
                if (_inform == null)
                { _inform = new Guardian(); }
                return _inform;
            }
        }


        public void IsTrue(bool test,
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            IsTrue(test, "", callerName, callerLineNumber);
        }

        public void IsTrue(bool test, string message,
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (!test) throw new NotImplementedException();
        }

     public void IsGreaterThan(int expected, int test,
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            IsGreaterThan(expected, test, "", callerName, callerLineNumber);
        }

        public void IsGreaterThan(int expected, int test, string message,
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (test <=expected) throw new NotImplementedException();
        }

        public void BadContainer()
        {
            throw new NotImplementedException();
        }

       ///// <summary>
        ///// Call to inform of an unexpected null. 
        ///// </summary>
        ///// <param name="value"></param>
        ///// <remarks>
        ///// Please do not call on non-null values because this results in boxing
        ///// </remarks>
        //public void IsNotNull<T>(
        //    T value,
        //    [CallerMemberName] string callerName = "",
        //    [CallerLineNumber] int callerLineNumber = 0)
        //    where T : class
        //{
        //}

        /// <summary>
        /// Call to inform of an unexpected null. 
        /// </summary>
        /// <param name="value">Value to check for null</param>
        /// <param name="name">Name of value where practical, generally retrieved via new nameof operator which has a noop implementation in most locations</param>
        /// <remarks>
        /// Please do not call on non-null values because this results in boxing
        /// </remarks>
        public void IsNotNull<T>(
            T value,
            string name,
            [CallerMemberName] string callerName = "",
            [CallerLineNumber] int callerLineNumber = 0)
            where T : class
        {
            if (value == null) throw new NotImplementedException();
        }


    }
}
