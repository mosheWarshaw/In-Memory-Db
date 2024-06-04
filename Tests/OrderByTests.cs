using InMemoryDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace InMemoryDbTests
{
    [TestClass]
    public class OrderByTests
    {
        [TestMethod]
        public void Test1()
        {
            Rows rows1 = new Rows();
            Table table1 = new Table(rows1);
            table1.Create<string>(Names.nameCol);
            table1.AddCells(Names.nameCol, "abc", "def", "ghi");

            Rows rows2 = new Rows();
            Table table2 = new Table(rows2);
            table2.Create<string>(Names.nameCol);
            table2.AddCells(Names.nameCol, "def", "abc", "ghi");

            Assert.IsTrue(rows1.Equals(rows2));
        }

        [TestMethod]
        public void Test2()
        {
            Rows rows1 = new Rows();
            Table table1 = new Table(rows1);

            table1.Create<string>(Names.nameCol);
            table1.AddCell(Names.nameCol, "xavier");
            table1.AddCell(Names.nameCol, "xavier");
            table1.AddCell(Names.nameCol, "moshe");
            table1.AddCell(Names.nameCol, "moshe");
            table1.AddCell(Names.nameCol, "moshe");


            table1.Create<string>(Names.letterCol);
            table1.AddCell(Names.letterCol, "e");
            table1.AddCell(Names.letterCol, "d");
            table1.AddCell(Names.letterCol, "b");
            table1.AddCell(Names.letterCol, "c");
            table1.AddCell(Names.letterCol, "a");


            table1.Create<int>(Names.ageCol);
            table1.AddCell(Names.ageCol, 4);
            table1.AddCell(Names.ageCol, 5);
            table1.AddCell(Names.ageCol, 2);
            table1.AddCell(Names.ageCol, 3);
            table1.AddCell(Names.ageCol, 1);




            Rows rows2 = new Rows();
            Table expectedTable = new Table(rows2);
            expectedTable.Create<string>(Names.nameCol);
            expectedTable.AddCells(Names.nameCol, "moshe", "moshe", "moshe", "xavier", "xavier");
            expectedTable.Create<int>(Names.ageCol);
            expectedTable.AddCells(Names.ageCol, 1, 2, 3, 4, 5);
            expectedTable.Create<string>(Names.letterCol);
            expectedTable.AddCells(Names.letterCol, "a", "b", "c", "e", "d");



            Assert.IsTrue(rows1.Equals(rows2));
        }
    }
}
