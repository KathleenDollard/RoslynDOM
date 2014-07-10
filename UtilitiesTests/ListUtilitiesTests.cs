using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;

namespace UtilitiesTests
{
    [TestClass]
    public class ListUtilitiesTests
    {
        private const string ListUtilitiesCategory = "ListUtilities";

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

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Can_create_list_of_item_ref_types()
        {
            var input = new TestClassA<string>(new string[] { "a", "b", "c" });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = x => x.List;
            Func<string, TestClassB<string>> makeItemDeleg = x => new TestClassB<string>(x.ToUpper());
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(3, members.Count());
            Assert.AreEqual("A", members.First().Item);
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Can_create_list_of_item_value_types()
        {
            var input = new TestClassA<int>(new int[] { 0, 1, 2 });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, TestClassB<int>> makeItemDeleg = x => new TestClassB<int>(x);
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(3, members.Count());
            Assert.AreEqual(2, members.Last().Item);
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_ref_type_item_list_on_empty_list()
        {
            var input = new TestClassA<string>(new string[] { });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = x => x.List;
            Func<string, TestClassB<string>> makeItemDeleg = x => new TestClassB<string>(x.ToUpper());
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_value_type_item_list_on_empty_list()
        {
            var input = new TestClassA<int>(new int[] { });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, TestClassB<int>> makeItemDeleg = x => new TestClassB<int>(x);
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_ref_type_item_list_on_null()
        {
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = x => x.List;
            Func<string, TestClassB<string>> makeItemDeleg = x => new TestClassB<string>(x.ToUpper());
            var members = ListUtilities.MakeList(null, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_value_type_item_list_on_null()
        {
            var input = new TestClassA<int>(new int[] { });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, TestClassB<int>> makeItemDeleg = x => new TestClassB<int>(x);
            var members = ListUtilities.MakeList(null, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeList_throws_on_null_getItem_delegate_for_item()
        {
            var input = new TestClassA<string>(new string[] { });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = null;
            Func<string, TestClassB<string>> makeItemDeleg = x => new TestClassB<string>(x.ToUpper());
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeList_throws_on_null_makeItem_delegate_for_item()
        {
            var input = new TestClassA<int>(new int[] { });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, TestClassB<int>> makeItemDeleg = null;
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Can_create_list_of_items_ref_types()
        {
            var input = new TestClassA<string>(new string[] { "a", "b", "c" });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = x => x.List;
            Func<string, IEnumerable<TestClassB<string>>> makeItemDeleg = x => new TestClassB<string>[] { new TestClassB<string>(x.ToUpper()) };
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(3, members.Count());
            Assert.AreEqual("A", members.First().Item);
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Can_create_list_of_items_value_types()
        {
            var input = new TestClassA<int>(new int[] { 0, 1, 2 });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, IEnumerable<TestClassB<int>>> makeItemDeleg = x => new TestClassB<int>[] { new TestClassB<int>(x) };
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(3, members.Count());
            Assert.AreEqual(2, members.Last().Item);
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_ref_type_items_list_on_empty_list()
        {
            var input = new TestClassA<string>(new string[] { });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = x => x.List;
            Func<string, IEnumerable<TestClassB<string>>> makeItemDeleg = x => new TestClassB<string>[] { new TestClassB<string>(x.ToUpper()) };
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_value_type_items_list_on_empty_list()
        {
            var input = new TestClassA<int>(new int[] { });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, IEnumerable<TestClassB<int>>> makeItemDeleg = x => new TestClassB<int>[] { new TestClassB<int>(x) };
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_ref_type_items_list_on_null()
        {
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = x => x.List;
            Func<string, IEnumerable<TestClassB<string>>> makeItemDeleg = x => new TestClassB<string>[] { new TestClassB<string>(x.ToUpper()) };
            var members = ListUtilities.MakeList(null, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        public void Returns_empty_value_type_items_list_on_null()
        {
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, IEnumerable<TestClassB<int>>> makeItemDeleg = x => new TestClassB<int>[] { new TestClassB<int>(x) };
            var members = ListUtilities.MakeList(null, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeList_throws_on_null_getItem_delegate_for_items()
        {
            var input = new TestClassA<string>(new string[] { });
            Func<TestClassA<string>, IEnumerable<string>> getItemsDeleg = null;
            Func<string, IEnumerable<TestClassB<string>>> makeItemDeleg = x => new TestClassB<string>[] { new TestClassB<string>(x.ToUpper()) };
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }

        [TestMethod, TestCategory(ListUtilitiesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeList_throws_on_null_makeItem_delegate_for_items()
        {
            var input = new TestClassA<int>(new int[] { });
            Func<TestClassA<int>, IEnumerable<int>> getItemsDeleg = x => x.List;
            Func<int, IEnumerable<TestClassB<int>>> makeItemDeleg = null;
            var members = ListUtilities.MakeList(input, getItemsDeleg, makeItemDeleg);

            Assert.AreEqual(0, members.Count());
        }
    }
}
