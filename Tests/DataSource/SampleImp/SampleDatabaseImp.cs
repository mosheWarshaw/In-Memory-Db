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
    public class SampleDatabaseImp : IDatabase<ITable>
    {
        private Dictionary<string, ITable> tables = new Dictionary<string, ITable>();

        public SampleDatabaseImp()
        {
        }

        public void AddTable(string name, ITable table)
        {
            tables[name] = table;
        }

        public Dictionary<string, ITable> GetTables()
        {
            return tables;
        }

        public Database GetDatabase()
        {
            return null;
        }
    }
}
