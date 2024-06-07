using System;

namespace InMemoryDb
{
    public partial class Table
    {
        private Rows _rows { get; }

        public Table() : this(new Rows())
        {
        }

        public Table(Rows rows)
        {
            //Deliberately not doing defensive copying.
            _rows = rows;
        }


        public void Create<T>(string columnName, int startingSize = 0, bool isNullable = false)
        {
            if (_rows.Contains(columnName))
            {
                throw new ArgumentException();
            }

            _rows.columns[columnName] = new Column<T>(startingSize, isNullable);
        }

        public void CreateFkColumn<T>(string columnName, string referencedTableName, string referencedColumnName, int startingSize = 0, bool isNullable = false)
        {
            _rows.columns[columnName] = new Column<T>(referencedTableName, referencedColumnName, startingSize, isNullable);
        }

        public void Add(string columnName, IColumn column)
        {
            if (_rows.Contains(columnName))
            {
                throw new ArgumentException();
            }

            _rows.columns[columnName] = column;
        }


        public void SetAddresses(Database db)
        {
            foreach (IColumn column in _rows.columns.Values)
            {
                column.SetIndexes(db);
            }
        }
    }
}