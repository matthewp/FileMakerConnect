using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileMakerConnect;

namespace TestFMConnect
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSelect()
        {
            FileMakerSelect cmd = new FileMakerSelect("connection string here");
            cmd.Select("Foo", "Bar", "Doo", "Bird");
            cmd.From("MyDB");
            cmd.Where("Foo", Condition.Equals, 37);
            cmd.Where("Bar", Condition.GreaterThan, "something");
            cmd.OrderBy("Doo", SortOrder.Descending);
            cmd.OrderBy("Foo", SortOrder.Ascending);

            string sql = cmd.BuildOutputString();
            FileMakerResult result = cmd.Execute();
            Foo foo = result.ExtractObject<Foo>();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestUpdate()
        {
            FileMakerUpdate cmd = new FileMakerUpdate("connection string here", "Table");
            cmd.Set("Foo", "bar");
            cmd.Set("Doo", 66);
            cmd.Set("When", DateTime.Now);
            cmd.Where("FooID", Condition.Equals, 1);
            cmd.Execute();

            Assert.IsTrue(true);
        }
    }

    public class Foo
    {
        public string Bar { get; set; }
    }
}
