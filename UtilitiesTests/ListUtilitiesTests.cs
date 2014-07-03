using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;

namespace UtilitiesTests
{
    [TestClass]
    public  class ListUtilitiesTests
    {

        private class TestClassA<T>
        {
            public IEnumerable<T> List;
            public TestClassA(IEnumerable<T> list)
            {
                List = list;
            }
        }
        private class TestClassB<T>
        {
            public T Item;
            public TestClassB(T item)
            {
                Item = item;
            }
        }
        [TestMethod]
        public void Can_create_list_of_ref_types()
        {
            var input = new TestClassA<string>(new string[] { "a", "b", "c" });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg =  x => x.List;
            Func<string, TestClassB<string>> makeItemDeleg =  x => new TestClassB<string>(x.ToUpper());
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(3, members.Count());
            Assert.AreEqual("A", members.First().Item);
        }

        [TestMethod]
        public void Can_create_list_of_value_types()
        {
            var input = new TestClassA<int>(new int[] { 0, 1, 2});
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, TestClassB<int>> makeItemDeleg = x => new TestClassB<int>(x);
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(3, members.Count());
            Assert.AreEqual(2, members.Last().Item);
        }

        [TestMethod]
        public void Returns_empty_ref_type_list()
        {
            var input = new TestClassA<string>(new string[] { });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = x => x.List;
            Func<string, TestClassB<string>> makeItemDeleg = x => new TestClassB<string>(x.ToUpper());
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod]
        public void Returns_empty_value_type_list()
        {
            var input = new TestClassA<int>(new int[] { });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, TestClassB<int>> makeItemDeleg = x => new TestClassB<int>(x);
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }
    }
}
