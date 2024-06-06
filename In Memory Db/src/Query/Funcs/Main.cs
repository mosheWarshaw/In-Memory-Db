﻿using System.Collections.Generic;

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
        /// For when you want to use query funtions without using a table or database.
        /// </summary>
        public Funcs(Rows rows)
        {
            _lastResult = rows;
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