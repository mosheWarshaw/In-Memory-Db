using System;

namespace InMemoryDb
{
    public interface IColumn
    {
        int GetSize();
        T GetCell<T>(int index);
        void AddCell<T>(T val);
        void SetCell<T>(int index, T val);
        IColumnWrapper GetColumnWrapper();
        T GetTempCell<T>();
        void Swap(int index1, int index2);
        void GetCell(int rowIndex, out dynamic val);
        Type GetColumnType();
    }
}
