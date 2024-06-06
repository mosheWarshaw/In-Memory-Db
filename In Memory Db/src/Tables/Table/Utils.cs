namespace InMemoryDb
{
    public partial class Table
    {
        public int GetNumOfRows()
        {
            foreach (KeyValuePair<string, IColumn> entry in _rows.columns)
            {
                return entry.Value.GetSize();
            }
            return 0;
        }

        public IColumnWrapper GetColumnWrapper(string columnName)
        {
            return _rows.columns[columnName].GetColumnWrapper();
        }

        public Dictionary<string, Type> GetColumnTypes()
        {
            Dictionary<string, Type> columnTypes = new();
            foreach (KeyValuePair<string, IColumn> entry in _rows.columns)
            {
                columnTypes[entry.Key] = entry.Value.GetColumnType();
            }
            return columnTypes;
        }

        public IEnumerable<string> GetColumnNames()
        {
            return _rows.columns.Keys;
        }

        public bool IsFk(string columnName)
        {
            return _rows.columns[columnName].IsFk();
        }

        public int GetIndexOfNth<T>(string columnName, T val, int n)
        {
            return _rows.columns[columnName].GetIndexOfNth(val, n);
        }

        public int? GetPkIndex(int fkIndex, string columnName)
        {
            return _rows.columns[columnName].GetPkIndex(fkIndex);
        }

        public bool Contains(string columnName)
        {
            return _rows.columns.ContainsKey(columnName);
        }
    }
}
