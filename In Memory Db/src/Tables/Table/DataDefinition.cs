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


        public void Create<T>(string columnName, int startingSize = 0)
        {
            if (_rows.Contains(columnName))
            {
                throw new ArgumentException();
            }

            _rows.columns[columnName] = new Column<T>(startingSize);
        }

        public void Add(string columnName, IColumn column)
        {
            if (_rows.Contains(columnName))
            {
                throw new ArgumentException();
            }

            _rows.columns[columnName] = column;
        }
    }
}