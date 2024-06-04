namespace in_memory_db
{
    public class ColumnWrapper<T> : IColumnWrapper
    {
        private readonly Column<T> _column;

        public ColumnWrapper(Column<T> column)
        {
            _column = column;
        }

        public T GetCell(int index)
        {
            return _column.GetCell<T>(index);
        }
    }
}
