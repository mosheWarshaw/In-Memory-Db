using System;

namespace InMemoryDb
{
    public interface IColumn
    {
        T GetCell<T>(int index);
        void GetCell(int rowIndex, out dynamic val);
        void AddCell<T>(T val);
        void SetCell<T>(int index, T val);

        T GetTempVal<T>();
        void AddTempVal();

        int GetSize();
        IColumnWrapper GetColumnWrapper();
        void Swap(int index1, int index2);

        //For adapter.
        Type GetColumnType();
    }
}
