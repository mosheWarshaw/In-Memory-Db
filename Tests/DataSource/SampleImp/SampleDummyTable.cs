using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using in_memory_db;
using in_memory_db.DataSource.DB;

namespace in_memory_db_tests.DataSource.SampleImp
{
    // Note: this class is solely for testing IO/Save. Instantiating one of these tables returns a table with no actual data stored in memory, only data that
    // will be returned with deferred execution. This way, we don't have to store a large amount of objects in storage and our unit tests
    // are only slowed down by the Save and Load methods they are actually testing. For testing correctness of the Save and Load methods,
    // use a different class that saves the data.
    public class SampleDummyTable : ITable
    {
        private Dictionary<string, Type> columnValueTypes = new Dictionary<string, Type>();
        public Dictionary<string, Type> ColumnValueTypes => columnValueTypes;
        private int numRows = 0;
        private List<string> columnsInOrder = new List<string>();

        public SampleDummyTable()
        {

        }

        public SampleDummyTable(int numRows)
        {
            this.numRows = numRows;
        }

        public void CreateColumn(string name, Type type)
        {
            if (type != typeof(int) && type != typeof(string))
                throw new NotImplementedException("only types string and int supported in this class right now");
            columnValueTypes[name] = type;
            columnsInOrder.Add(name);
        }

        public void AddRow(IEnumerable<dynamic> values)
        {
            // dummy table, so we don't actually do anything here
            numRows++;
            System.Diagnostics.Debug.WriteLine($"added row #{numRows}");
        }

        public void CreateIndex(IEnumerable<string> columns, bool unique)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<dynamic>> GetRows()
        {
            for (int j = 0; j < numRows; j++)
            {
                dynamic[] row = new dynamic[columnValueTypes.Count];
                for (int i = 0; i < row.Length; i++)
                {
                    Type columnType = columnValueTypes[columnsInOrder[i]];
                    if (columnType == typeof(int))
                    {
                        row[i] = i;
                    }
                    else
                    {
                        row[i] = i.ToString();
                    }
                }
                yield return row;
            }
        }

        // Set the number of rows in this table. All rows are automatically 'populated' when they are returned as this 
        // table isn't storing real data.
        public void SetNumRows(int num)
        {
            numRows = num;
        }        

        public void AddCell<T>(string columnName, T val)
        {

        }
    }
}
