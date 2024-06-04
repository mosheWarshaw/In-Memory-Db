using in_memory_db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using in_memory_db.DataSource.DB;
using in_memory_db.DataSource.IO;
using in_memory_db_tests.DataSource.SampleImp;

namespace in_memory_db_tests.DataSource
{
    [TestClass]
    public class DataSourceTests
    {
        private readonly string IO_DIR = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Tests\\DataSource\\DataSourceOutput\\";

        // todo test that table saved and loaded are equal

        [TestMethod]
        public void TestSave1()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            Uri uri = new Uri(IO_DIR + "testSave1.dat");
            IDatabase<ITable> db = GetSampleDatabaseDeferred(5, new[] { typeof(int), typeof(string), typeof(int), typeof(string), typeof(int), typeof(string), typeof(int), typeof(string), typeof(int), typeof(string), }, 100000);
            timer.Stop();
            Console.WriteLine("creating databases took " + timer.ElapsedMilliseconds + " ms");
            timer.Restart();
            var success = Save.ToFile(uri, db);
            Console.WriteLine("saving to file took " + timer.ElapsedMilliseconds + " ms");
            Assert.IsTrue(success);

            // 5GB db:
            // creating databases took 6 ms
            // saving to file took 178477 ms (3 min)
        }

        [TestMethod]
        public void TestLoad1()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            Uri uri = new Uri(IO_DIR + "testSave1.dat");
            SampleDatabaseImp db = Load.FromFile<SampleDatabaseImp, SampleDummyTable>(uri);
            timer.Stop();
            Console.WriteLine("loading from file took " + timer.ElapsedMilliseconds + " ms");
            //PrintDatabase(db);
        }


        private static IDatabase<ITable> GetSampleDatabaseDeferred(int numTables, Type[] columnTypes, int numRows)
        {
            // NOTE: this method currently ONLY SUPPORTs int and string columnTypes for now (todo update)
            IDatabase<ITable> db = new SampleDatabaseImp();

            for (int i = 0; i < numTables; i++)
            {
                ITable table = new SampleDummyTable(numRows);
                for (int j = 0; j < columnTypes.Length; j++)
                {
                    table.CreateColumn($"col{j}", columnTypes[j]);
                }

                db.AddTable(i.ToString(), table);
            }

            return db;
        }

        private static IDatabase<ITable> GetSampleDatabase(int numTables, string[] columnNames, Type[] columnTypes, int numRows)
        {
            // NOTE: this method currently ONLY SUPPORTs int and string columnTypes for now (todo update)
            IDatabase<ITable> db = new SampleDatabaseImp();

            for (int i = 0; i < numTables; i++)
            {
                ITable table = new SampleTableImp();
                for (int j = 0; j < columnTypes.Length; j++)
                {
                    table.CreateColumn($"col{j}", columnTypes[j]);
                }

                for (int k = 0; k < columnTypes.Length; k++)
                {
                    for (int j = 0; j < numRows; j++)
                    {
                        if (columnTypes[k] == typeof(int))
                        {
                            table.AddCell(columnNames[k], k);
                        }
                        else
                        {
                            table.AddCell(columnNames[k], k.ToString());
                        }
                    }
                }

                db.AddTable(i.ToString(), table);
            }

            return db;
        }

        private static void PrintDatabase(IDatabase<ITable> db)
        {
            // todo, not currently in matching format to Save.ToFile() - add option to Save class to get output in text, and use that
            // here. For now, it is a similar format but not exact.
            foreach (var tableEntry in db.GetTables())
            {
                string tableName = tableEntry.Key;
                ITable table = tableEntry.Value;
                Console.Write($"TABLE_NAME[{tableName}]\nTABLE_COLUMNS[");
                foreach (var column in table.ColumnValueTypes)
                {
                    Console.Write($"[{column.Key}:{column.Value}]_");
                }

                Console.WriteLine("]");

                foreach (var row in table.GetRows())
                {
                    foreach (var val in row)
                    {
                        Console.Write($"[{val}:{val.GetType()}]_");
                    }

                    Console.WriteLine();
                }

                Console.Write('+'.ToString());
            }
            Console.Write("\0\n");
        }
    }
}