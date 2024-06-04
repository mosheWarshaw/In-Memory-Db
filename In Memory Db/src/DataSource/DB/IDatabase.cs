using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDb.DataSource.DB
{
    // todo make this a DTO?
    public interface IDatabase<T>
        where T : ITable
    {
        Database GetDatabase();
        /// <summary>
        /// Adds a table to this database
        /// </summary>
        /// <param name="table">The table to add</param>
        public abstract void AddTable(string name, T table);

        public abstract Dictionary<string, T> GetTables();  // maps tableName -> table
    }
}