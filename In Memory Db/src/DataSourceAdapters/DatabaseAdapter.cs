using InMemoryDb.DataSource.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDb.DataSourceAdapters
{
    public class DatabaseAdapter : IDatabase<ITable>
    {
        private readonly Database _database;

        public DatabaseAdapter()
        {
            _database = new Database();
        }

        public DatabaseAdapter(Database db)
        {
            _database = db;
        }

        public Database GetDatabase()
        {
            return _database;
        }

        public void AddTable(string name, ITable table)
        {
            if (table is TableAdapter)
            {
                _database.Add(name, ((TableAdapter) table).GetTable());
            }
            else
            {
                throw new ArgumentException($"table must be of type TableAdapter");
            }
        }

        public Dictionary<string, ITable> GetTables()
        {
            Dictionary<string, ITable> tables = new();
            foreach(KeyValuePair<string, Table> entry in _database.GetTables())
            {
                tables.Add(entry.Key, new TableAdapter(entry.Value));
            }
            return tables;
        }
    }
}
