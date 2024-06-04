using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace in_memory_db
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
    }
}
