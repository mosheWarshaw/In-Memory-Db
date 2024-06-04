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
        /// For when you want to use query funtions without using a table or database.
        /// </summary>
        public Funcs(Rows rows)
        {
            _lastResult = rows;
        }


        public RowsWrapper GetResult()
        {
            return new RowsWrapper(_lastResult);
        }
    }
}