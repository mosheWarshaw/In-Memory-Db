using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace in_memory_db.DataSource.DB
{
    // todo make this a DTO?
    public interface ITable
    {
        public abstract void CreateColumn(string name, Type type);

        public abstract void CreateIndex(IEnumerable<string> columns, bool unique);  // todo add support for indexes to Save and Load

        public abstract void AddCell<T>(string columnName, T value);

        public abstract IEnumerable<IEnumerable<dynamic>> GetRows();

        public abstract Dictionary<string, Type> ColumnValueTypes { get; }
    }
}