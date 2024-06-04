using in_memory_db.DataSource.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace in_memory_db.DataSourceAdapters
{
    public class TableAdapter : ITable
    {
        private readonly Table _table;

        public TableAdapter()
        {
            _table = new Table();
        }

        public TableAdapter(Table table)
        {
            _table = table;
        }

        public Dictionary<string, Type> ColumnValueTypes
        {
            get
            {
                return _table.GetColumnTypes();
            }
        }

        public void AddCell<T>(string columnName, T val)
        {
            _table.AddCell(columnName, val);
        }

        //These jsut contain the current types listed in Load's constructor. Add nullable types.
        public void CreateColumn(string name, Type type)
        {
            // todo use a map of _table.Create method references for each type instead of the if-else's. same as Load.ReadValuesIntoTable() does
            if (type == typeof(string))
            {
                _table.Create<string>(name);
            }
            else if (type == typeof(char))
            {
                _table.Create<char>(name);
            }
            else if (type == typeof(bool))
            {
                _table.Create<bool>(name);
            }
            else if (type == typeof(decimal))
            {
                _table.Create<decimal>(name);
            }
            else if (type == typeof(double))
            {
                _table.Create<double>(name);
            }
            else if (type == typeof(long))
            {
                _table.Create<long>(name);
            }
            else if (type == typeof(int))
            {
                _table.Create<int>(name);
            }
            else if (type == typeof(uint))
            {
                _table.Create<uint>(name);
            }
            else if (type == typeof(short))
            {
                _table.Create<short>(name);
            }
            else if (type == typeof(ushort))
            {
                _table.Create<ushort>(name);
            }
            else if (type == typeof(byte))
            {
                _table.Create<byte>(name);
            }
            else if (type == typeof(sbyte))
            {
                _table.Create<sbyte>(name);
            }
            else
            {
                throw new NotImplementedException($"type {type} not supported");
            }
        }

        public void CreateIndex(IEnumerable<string> columns, bool unique)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<dynamic>> GetRows()
        {
            List<List<dynamic>> rows = new();

            dynamic cell;
            List<dynamic> list;
            int numOfRows = _table.GetNumOfRows();
            IEnumerable<string> columnNames = _table.GetColumnNames();
            for (int i = 0; i < numOfRows; i++)
            {
                list = new();
                foreach (string name in columnNames)
                {
                    _table.GetCell(i, name, out cell);
                    list.Add(cell);
                }
                rows.Add(list);
            }

            return rows;
        }

        public Table GetTable()
        {
            return _table;
        }
    }
}
