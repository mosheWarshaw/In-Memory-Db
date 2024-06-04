using System;
using System.Collections.Generic;
namespace InMemoryDb
{
    public class Databases
    {
        private Dictionary<string, Database> _databases = new Dictionary<string, Database>();
        public void Create(string dbName)
        {
            if (_databases.ContainsKey(dbName))
                throw new ArgumentException();

            _databases[dbName] = new Database();
        }

        public Database Get(string dbName)
        {
            return _databases[dbName];
        }

        public void Drop(string dbName)
        {
            _databases.Remove(dbName);
        }
    }
}