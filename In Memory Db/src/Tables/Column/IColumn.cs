using System;

namespace InMemoryDb
{
    public interface IColumn
    {
        bool IsFk();

        T GetCell<T>(int index);
        void GetCell(int rowIndex, out dynamic val);
        void AddCell<T>(T val);
        void SetCell<T>(int index, T val);

        T GetTempVal<T>();
        void AddTempVal();


        int? GetPkIndex(int indexOfFk);

        int GetIndexOfNth<T>(T val, int n);
        void SetIndexes(Database db);
        int GetSize();
        IColumnWrapper GetColumnWrapper();
        void Swap(int index1, int index2);

        //For adapter.
        Type GetColumnType();
    }
}
