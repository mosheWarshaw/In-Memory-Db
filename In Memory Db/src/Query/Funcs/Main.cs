using System.Collections.Generic;

namespace InMemoryDb
{
    public partial class Funcs
    {
        private Database _database { get; }
        private Rows _currResultRows;
        private Table _resultTable;
        private Dictionary<string, Rows> _results = new Dictionary<string, Rows>();
        private Rows _lastResult;


        public Funcs(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// For when you want to use query functions without using a table or database.
        /// </summary>
        public Funcs(Rows rows)
        {
            _lastResult = rows;
        }





        public void _SetUpFunc(ref Func<SameRowAccessor, bool> where)
        {
            _currResultRows = new Rows();
            _resultTable = new Table(_currResultRows);
            if (where == null)
                where = row => true;
        }

        public void ColsSetUp(string tableName, out Table sourceTable, ICol[] cols, SameRowAccessor sameRowAccessor)
        {
            sourceTable = _database.Contains(tableName) ? _database.Get(tableName) : new Table(_results[tableName]);
            foreach (ICol col in cols)
            {
                col.SetSourceColumnWrapper(sourceTable.GetColumnWrapper(col.SourceColumnName));
                col.SetSameRowAccessor(sameRowAccessor);
                IColumn column = col.GetColumn();
                _resultTable.Add(col.ResultColumnName, column);
            }
        }

        public void _EndOfFunc(string nameOfResultTable)
        {
            if (nameOfResultTable != null)
            {
                _results[nameOfResultTable] = _currResultRows;
            }
            _lastResult = _currResultRows;
            _currResultRows = null;
            _resultTable = null;
        }





        #region screening
        private void _ScreenCols(params ICol[][] cols)
        {
            foreach (ICol[] colArr in cols)
            {
                if (colArr.Length == 0)
                    throw new ArgumentException();
            }
        }

        private void _ScreenExistingColumnNames(Database db, params string[][] columnNames)
        {
            string tableName;
            string columnName;
            foreach (string[] aColumnName in columnNames)
            {
                tableName = aColumnName[0];
                columnName = aColumnName[1];
                if (!db.Get(tableName).Contains(columnName))
                    throw new ArgumentException();
            }
        }

        private void _ScreenNewTableNames(params string[] tableNames)
        {
            Func<string, bool> alreadyExists = tableName => _database.Contains(tableName) || _results.ContainsKey(tableName);
            _ScreenTableNames(alreadyExists, tableNames);
        }

        private void _ScreenExistingTableNames(params string[] tableNames)
        {
            Func<string, bool> doesntExist = tableName => !_database.Contains(tableName) && !_results.ContainsKey(tableName);
            _ScreenTableNames(doesntExist, tableNames);
        }

        private void _ScreenTableNames(Func<string, bool> screen, params string[] tableNames)
        {
            foreach (string tableName in tableNames)
            {
                if (screen(tableName))
                    throw new ArgumentException();

            }
        }
        #endregion






        /// <summary>
        /// For when there is only one cell in the results,
        /// and having a new RowsWrapper returned would be wasteful, such
        /// as when running many Select queries in Column.SetAddresses.
        /// </summary>
        public T GetScalarValue<T>()
        {
            string columnName = _lastResult.columns.First().Key;
            return _lastResult.columns[columnName].GetCell<T>(0);
        }

        public RowsWrapper GetResult()
        {
            return new RowsWrapper(_lastResult);
        }
    }
}