using InMemoryDb;
using InMemoryDb.src.Query.Col;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

//OrderBy is used in all these select tests. It is used in detemringing Rows equality.

namespace InMemoryDbTests
{
    [TestClass]
    public class SelectAndWhereTests
    {
        [TestMethod]
        public void Test1()
        {
            #region actual
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);
            db.Create(Names.table1);
            Table actualTable = db.Get(Names.table1);

            actualTable.Create<string>(Names.strcol);
            actualTable.AddCell(Names.strcol, "abc");
            actualTable.AddCell(Names.strcol, "def");
            actualTable.AddCell(Names.strcol, "ghi");


            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select
                (
                    Names.table1,
                    new ICol[] { new Col<string, string>(Names.strcol) }
                ).
                GetResult();
            #endregion

            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<string>(Names.strcol);
            expectedTable.AddCells(Names.strcol, "def", "abc", "ghi");
            #endregion

            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }


        [TestMethod]
        public void Test2()
        {
            #region actual
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);
            db.Create(Names.table1);
            Table actualTable = db.Get(Names.table1);

            actualTable.Create<string>(Names.nameCol);
            actualTable.AddCell(Names.nameCol, "moshe");
            actualTable.AddCell(Names.nameCol, "xavier");


            actualTable.Create<int>(Names.ageCol);
            actualTable.AddCell(Names.ageCol, 21);
            actualTable.AddCell(Names.ageCol, 21);


            actualTable.Create<string>(Names.letterCol);
            actualTable.AddCell(Names.letterCol, "b");
            actualTable.AddCell(Names.letterCol, "a");


            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select
                (
                    Names.table1,
                    new ICol[]
                    {
                        new Col<string, string>(Names.nameCol),
                        new Col<int, int>(Names.ageCol),
                        new Col<string, string>(Names.letterCol)
                    }
                ).
                GetResult();
            #endregion




            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<string>(Names.nameCol);
            expectedTable.AddCells(Names.nameCol, "moshe", "xavier");
            expectedTable.Create<int>(Names.ageCol);
            expectedTable.AddCells(Names.ageCol, 21, 21);
            expectedTable.Create<string>(Names.letterCol);
            expectedTable.AddCells(Names.letterCol, "b", "a");
            #endregion



            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }


        [TestMethod]
        public void Test3()
        {
            #region actual
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);
            db.Create(Names.table1);
            Table actualTable = db.Get(Names.table1);

            actualTable.Create<string>(Names.nameCol);
            actualTable.AddCell(Names.nameCol, "xavier");
            actualTable.AddCell(Names.nameCol, "moshe");


            actualTable.Create<int>(Names.ageCol);
            actualTable.AddCell(Names.ageCol, 21);
            actualTable.AddCell(Names.ageCol, 21);


            actualTable.Create<string>(Names.letterCol);
            actualTable.AddCell(Names.letterCol, "a");
            actualTable.AddCell(Names.letterCol, "b");


            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select
                (
                    Names.table1,
                    new ICol[]
                    {
                        new Col<string, string>(Names.nameCol),
                        new Col<int, int>(Names.ageCol),
                        new Col<string, string>(Names.letterCol)
                    }
                ).
                GetResult();
            #endregion




            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<string>(Names.nameCol);
            expectedTable.AddCells(Names.nameCol, "moshe", "xavier");
            expectedTable.Create<int>(Names.ageCol);
            expectedTable.AddCells(Names.ageCol, 21, 21);
            expectedTable.Create<string>(Names.letterCol);
            expectedTable.AddCells(Names.letterCol, "b", "a");
            #endregion



            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }




        [TestMethod]
        public void Test4()
        {
            #region actual
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);
            db.Create(Names.table1);
            Table actualTable = db.Get(Names.table1);

            actualTable.Create<string>(Names.nameCol);
            actualTable.AddCell(Names.nameCol, "xavier");
            actualTable.AddCell(Names.nameCol, "moshe");


            actualTable.Create<int>(Names.ageCol);
            actualTable.AddCell(Names.ageCol, 4);
            actualTable.AddCell(Names.ageCol, 5);


            Func<int, SameRowAccessor, string> ageAction = (sourceCell, row) =>
               row.GetCell<string>("name") + (2 * sourceCell)
            ;
            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select
                (
                    Names.table1,
                    new ICol[]
                    {
                        new Col<string, string>(Names.nameCol),
                        new Col<int, string>(Names.ageCol, "updatedAge", ageAction)
                    }
                ).
                GetResult();
            #endregion




            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<string>(Names.nameCol);
            expectedTable.AddCells(Names.nameCol, "xavier", "moshe");
            expectedTable.Create<string>("updatedAge");
            expectedTable.AddCells("updatedAge", "xavier8", "moshe10");
            #endregion



            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }



        [TestMethod]
        public void Test5()
        {
            #region actual
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);
            db.Create(Names.table1);
            Table actualTable = db.Get(Names.table1);

            actualTable.Create<int>(Names.ageCol);
            actualTable.AddCell(Names.ageCol, 4);
            actualTable.AddCell(Names.ageCol, 5);
            actualTable.AddCell(Names.ageCol, 67);
            actualTable.AddCell(Names.ageCol, 1);


            Func<int, SameRowAccessor, int> ageAction = (sourceCell, row) =>
               2 * sourceCell
            ;
            Func<SameRowAccessor, bool> where = row => row.GetCell<int>("updatedAge") > 8;
            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select
                (
                    Names.table1,
                    new ICol[]
                    {
                        new Col<int, int>(Names.ageCol, "updatedAge", ageAction)
                    },
                    where
                ).
                GetResult();
            #endregion




            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<int>("updatedAge");
            expectedTable.AddCells("updatedAge", 10, 134);
            #endregion



            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }


        [TestMethod]
        public void Test6()
        {
            #region actual
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);
            db.Create(Names.table1);
            Table actualTable = db.Get(Names.table1);

            actualTable.Create<string>(Names.nameCol);
            actualTable.AddCell(Names.nameCol, "dont add 1");
            actualTable.AddCell(Names.nameCol, "moshe1");
            actualTable.AddCell(Names.nameCol, "moshe2");
            actualTable.AddCell(Names.nameCol, "dont add 2");

            actualTable.Create<int>(Names.ageCol);
            actualTable.AddCell(Names.ageCol, 4);
            actualTable.AddCell(Names.ageCol, 5);
            actualTable.AddCell(Names.ageCol, 67);
            actualTable.AddCell(Names.ageCol, 1);


            Func<int, SameRowAccessor, int> ageAction = (sourceCell, row) =>
               2 * sourceCell
            ;
            Func<SameRowAccessor, bool> where = row => row.GetCell<int>("updatedAge") > 8;
            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select
                (
                    Names.table1,
                    new ICol[]
                    {
                        new Col<string, string>(Names.nameCol),
                        new Col<int, int>(Names.ageCol, "updatedAge", ageAction)
                    },
                    where
                ).
                GetResult();
            #endregion




            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<string>(Names.nameCol);
            expectedTable.AddCells(Names.nameCol, "moshe1", "moshe2");
            expectedTable.Create<int>("updatedAge");
            expectedTable.AddCells("updatedAge", 10, 134);
            #endregion



            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }



        [TestMethod]
        public void Test7()
        {
            #region actual1
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);
            db.Create(Names.table1);
            Table actualTable1 = db.Get(Names.table1);

            actualTable1.Create<string>(Names.nameCol);
            actualTable1.AddCell(Names.nameCol, "abc");
            actualTable1.AddCell(Names.nameCol, "def");
            actualTable1.AddCell(Names.nameCol, "ghi");
            #endregion

            #region actual2
            db.Create(Names.table2);
            Table actualTable2 = db.Get(Names.table2);

            actualTable2.Create<int>(Names.ageCol);
            actualTable2.AddCell(Names.ageCol, 1);
            actualTable2.AddCell(Names.ageCol, 34);
            actualTable2.AddCell(Names.ageCol, 21);


            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select
                (
                    Names.table1,
                    new ICol[] { new Col<string, string>(Names.nameCol) }
                ).
                Select(
                    Names.table2,
                    new ICol[] { new Col<int, int>(Names.ageCol) }
                ).
                GetResult();
            #endregion



            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<int>(Names.ageCol);
            expectedTable.AddCells(Names.ageCol, 1, 34, 21);
            #endregion

            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }



        [TestMethod]
        public void Test8()
        {
            #region actual1
            Databases dbs = new Databases();
            dbs.Create(Names.db1);
            Database db = dbs.Get(Names.db1);

            db.Create(Names.table1);
            Table actualTable1 = db.Get(Names.table1);

            actualTable1.Create<int>(Names.ageCol);
            actualTable1.AddCell(Names.ageCol, 1);
            actualTable1.AddCell(Names.ageCol, 34);
            actualTable1.AddCell(Names.ageCol, 21);
            #endregion

            #region actual2
            db.Create(Names.table2);
            Table actualTable2 = db.Get(Names.table2);

            actualTable2.Create<string>(Names.nameCol);
            actualTable2.AddCell(Names.nameCol, "abc");
            actualTable2.AddCell(Names.nameCol, "def");
            actualTable2.AddCell(Names.nameCol, "ghi");


            Func<SameRowAccessor, bool> where = row => row.GetCell<int>(Names.ageCol) > 1;

            RowsWrapper actualRowsWrapper = new Funcs(db).
                Select(
                    Names.table1,
                    new ICol[] { new Col<int, int>(Names.ageCol) },
                    nameOfResultTable: "ageTable"
                ).
                Select
                (
                    Names.table2,
                    new ICol[] { new Col<string, string>(Names.nameCol) }
                ).
                Select
                (
                    "ageTable",
                    new ICol[] { new Col<int, int>(Names.ageCol)},
                    where
                ).
                GetResult();
            #endregion



            #region expected
            Rows expectedRows = new Rows();
            Table expectedTable = new Table(expectedRows);
            expectedTable.Create<int>(Names.ageCol);
            expectedTable.AddCells(Names.ageCol, 34, 21);
            #endregion

            Assert.IsTrue(actualRowsWrapper.RowsAreEqualTo(expectedRows));
        }
    }
}
