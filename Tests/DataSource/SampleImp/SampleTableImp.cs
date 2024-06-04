using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using in_memory_db;
using in_memory_db.DataSource.DB;

namespace in_memory_db_tests.DataSource.SampleImp
{
    // Note: this class is solely for testing IO/Load and IO/Save.
    public class SampleTableImp : ITable
    {
        private List<IEnumerable<dynamic>> rows = new List<IEnumerable<dynamic>>();
        private Dictionary<string, Type> columnValueTypes = new Dictionary<string, Type>();
        public Dictionary<string, Type> ColumnValueTypes => columnValueTypes;

        public SampleTableImp()
        {

        }

        public void CreateColumn(string name, Type type)
        {
            columnValueTypes[name] = type;
        }

        public void AddRow(IEnumerable<dynamic> values)
        {
            rows.Add(values);  // in a real implementation you would probably want to copy the values in case the IEnumerable's source changes, but we're not concerned with that here
        }

        public void CreateIndex(IEnumerable<string> columns, bool unique)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<dynamic>> GetRows()
        {
            foreach (IEnumerable<dynamic> row in rows)
            {
                yield return row;
            }
        }

        public void AddCell<T>(string columnName, T val)
        {

        }
    }
}
