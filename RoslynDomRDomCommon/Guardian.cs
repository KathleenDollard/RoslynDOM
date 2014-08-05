﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

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

        public void AccessedProviderBeforeInitialization(Type type)
        {
            throw new InvalidOperationException();
        }

        public void BadContainer()
        {
            throw new InvalidOperationException();
        }

        public void FactoryExists(IRDomFactory factory, Type type, IDom item)
        {
            if (factory == null)
            { throw new InvalidOperationException(); }
        }

        public void RDomHasCloneContructor(ConstructorInfo constructor, Type type)
        {
            if (constructor == null)
            { throw new InvalidOperationException("Missing constructor for clone"); }
        }

        public void NeitherCreateFromNorListOverridden<TKind>(Type type, SyntaxNode item)
            { throw new InvalidOperationException(); }

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
        /// Please do not call on value types because this results in boxing
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

     

        internal void FactorySetExists(FactorySet factorySet, Type kind, string v)
        {
            if (factorySet == null)
            { throw new InvalidOperationException(); }
        }

        internal void FactoryNotFound(IDom item)
        {
            throw new InvalidOperationException();
        }
    }
}