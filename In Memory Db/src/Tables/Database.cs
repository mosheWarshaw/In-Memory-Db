using System;
using System.Collections.Generic;

namespace in_memory_db
{
    public class Database
    {
        private Dictionary<string, Table> _tables = new Dictionary<string, Table>();

        public bool Contains(string tableName)
        {
            return _tables.ContainsKey(tableName);
        }

        public void Create(string tableName)
        {
            if (_tables.ContainsKey(tableName))
            {
                throw new ArgumentException();
            }
            _tables[tableName] = new Table();
        }

        public void Add(string tableName, Table table)
        {
            if (_tables.ContainsKey(tableName))
            {
                throw new ArgumentException();
            }
            _tables[tableName] = table;  // todo this isn't using the passed-in table?
        }

        public bool Drop(string tableName)
        {
            if (!_tables.ContainsKey(tableName))
            {
                throw new ArgumentException();
            }
            return _tables.Remove(tableName);
        }

        public Table Get(string tableName)
        {
            return _tables[tableName];
        }

        public bool Rename(string oldTableName, string newTableName)
        {
            if (!_tables.ContainsKey(oldTableName))
            {
                throw new ArgumentException();
            }

            _tables[newTableName] = _tables[oldTableName];
            return _tables.Remove(oldTableName);
        }

        //Temporary. For DatabaseAdapter only.
        public IEnumerable<KeyValuePair<string, Table>> GetTables()
        {
            List<KeyValuePair<string, Table>> tables = new();
            foreach (KeyValuePair<string, Table> entry in _tables)
            {
                tables.Add(entry);
            }
            return tables;
        }
    }
}