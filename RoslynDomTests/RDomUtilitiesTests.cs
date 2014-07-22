using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class RDomUtilitiesTests
    {
        [TestMethod]
        public void Can_create_navigate_and_modify_list()
        {
            var csharpCode = @"
                        public class Foo
                        {}
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var cl = root.Classes.First();
            var list = new RDomList<IClass>(root);
            list.AddOrMove(cl);
            for (int i = 0; i < 5; i++)
            {
                var newCl = cl.Copy();
                newCl.Name = cl.Name + i.ToString().Trim();
                list.AddOrMove(newCl);
            }
            // Can add correctly?
            Assert.AreEqual(6, list.Count());

            // Can loop
            var sb = new StringBuilder();
            foreach (var foo in list)
            { sb.AppendLine(foo.Name); }
            var expected = "Foo\r\nFoo0\r\nFoo1\r\nFoo2\r\nFoo3\r\nFoo4\r\n";
            Assert.AreEqual(expected, sb.ToString());

            // Can use LINQ 
            var foo3 = list.Skip(4).FirstOrDefault();
            Assert.AreEqual("Foo3", foo3.Name);

            // Parent is set
            Assert.AreEqual(root, foo3.Parent);

            // Can remove
            list.Remove(foo3);
            Assert.AreEqual("Foo4", list.Skip(4).FirstOrDefault().Name);

            // Parent is unset
            Assert.AreEqual(null, foo3.Parent);

            // Can insert
            list.InsertOrMove(2, foo3);
            var test = list.Skip(2).First();
            Assert.AreEqual(foo3, test);

            // Can insert before
            var fooA = cl.Copy();
            fooA.Name = "FooA";
            list.InsertOrMoveBefore(foo3, fooA);
            test = list.Skip(2).First();
            Assert.AreEqual(fooA, test);

            // Can insert after
            var fooB = cl.Copy();
            fooB.Name = "FooB";
            list.InsertOrMoveAfter(foo3, fooB);
            test = list.Skip(4).First();
            Assert.AreEqual(fooB, test);

            // Insert after last simply adds at end
            var pos = list.Count() + 5;
            var fooC = cl.Copy();
            fooC.Name = "FooC";
            list.InsertOrMove(pos, fooC);
            Assert.AreEqual(fooC, list.Last());

            // AddRange
            var fooD = cl.Copy(); fooD.Name = "FooD";
            var fooE = cl.Copy(); fooE.Name = "FooE";
            var fooF = cl.Copy(); fooF.Name = "FooF";
            list.AddOrMoveRange(new IClass[] { fooD, fooE, fooF });
            Assert.AreEqual(12, list.Count());

            // Can loop
            sb = new StringBuilder();
            foreach (var foo in list)
            { sb.AppendLine(foo.Name); }
            expected = "Foo\r\nFoo0\r\nFooA\r\nFoo3\r\nFooB\r\nFoo1\r\nFoo2\r\nFoo4\r\nFooC\r\nFooD\r\nFooE\r\nFooF\r\n";
            Assert.AreEqual(expected, sb.ToString());


        }

    }
}
