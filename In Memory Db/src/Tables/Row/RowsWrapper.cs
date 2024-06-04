using System;

namespace InMemoryDb
{    
    public class RowsWrapper
    {
        private Rows _rows { get; set; }

        public RowsWrapper(Rows rows)
        {
            _rows = rows;
        }

        public T GetCell<T>(int rowIndex, string columnName)
        {
            return _rows.GetCell<T>(rowIndex, columnName);
        }

        public void Print()
        {
            foreach (string columnName in _rows.columns.Keys)
            {
                Console.Write($"{columnName}\t");
            }
            Console.WriteLine();
            int numOfRows = new Table(_rows).GetNumOfRows();
            dynamic cell;
            for (int i = 0; i < numOfRows; i++)
            {
                foreach (IColumn column in _rows.columns.Values)
                {
                    column.GetCell(i, out cell);
                    Console.Write($"{cell}\t\t");
                }
                Console.WriteLine();
            }
        }

        #region For testing.
        public bool RowsAreEqualTo(RowsWrapper rowsWrapper)
        {
            return RowsAreEqualTo(rowsWrapper._rows);
        }

        public bool RowsAreEqualTo(Rows rows)
        {
            return _rows.Equals(rows);
        }
        #endregion
    }
}
