namespace InMemoryDb
{
    public class Rows
    {
        public Dictionary<string, IColumn> columns = new Dictionary<string, IColumn>();


        public T GetCell<T>(int rowIndex, string columnName)
        {
            return columns[columnName].GetCell<T>(rowIndex);
        }


        public void GetCell(int rowIndex, string columnName, out dynamic val)
        {
            columns[columnName].GetCell(rowIndex, out val);
        }


        public bool Contains(string columnName)
        {
            return columns.ContainsKey(columnName);
        }


        public override bool Equals(object obj)
        {
            Rows otherRows = (Rows)obj;
            Dictionary<string, IColumn> otherColumns = otherRows.columns;

            string[] columnNames = new string[columns.Count];
            int i = 0;

            foreach (KeyValuePair<string, IColumn> entry in columns)
            {
                string columnName = entry.Key;
                IColumn column = entry.Value;
                if (! otherColumns.ContainsKey(columnName) || column.GetSize() != otherColumns[columnName].GetSize())
                {
                    return false;
                }

                columnNames[i] = columnName;
                i++;
            }

            new Funcs(this).OrderBy(columnNames);
            new Funcs(otherRows).OrderBy(columnNames);
            /*Because of the ordering, now you can compare it row by row, else these 2
             * having the same rows but jsut in a differnet order would be considered not equal.*/
            bool equals;
            foreach (KeyValuePair<string, IColumn> entry in columns)
            {
                string columnName = entry.Key;
                IColumn column = entry.Value;
                IColumn otherColumn = otherColumns[columnName];
                equals = column.Equals(otherColumn);
                if (!equals)
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            //todo Because you're overriding the Equals method.
            return -1;
        }
    }
}