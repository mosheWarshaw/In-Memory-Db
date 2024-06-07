using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InMemoryDb.Funcs;

namespace InMemoryDb
{
    public partial class Table
    {

        public void GetCell(int rowIndex, string columnName, out dynamic val)  // AS: todo why is this not just returning the value?
        {
            _rows.GetCell(rowIndex,columnName, out val);
        }



        public void AddCell<T>(string columnName, T val)
        {
            _rows.columns[columnName].AddCell(val);
        }

        public void AddCells<T>(string columnName, params T[] vals)
        {
            foreach (T val in vals)
            {
                AddCell(columnName, val);
            }
        }


        //todo In future: You'll need to add the cascading of the foreign keys being changed when you add that part to the db.
        public void Swap(int index1, int index2)
        {
            foreach (KeyValuePair<string, IColumn> entry in _rows.columns)
            {
                IColumn column = entry.Value;
                column.Swap(index1, index2);
            }
        }
    }
}
