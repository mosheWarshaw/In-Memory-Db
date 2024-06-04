using System;
using System.Collections.Generic;
using System.Linq;

//the reason for the if statements in ehac ethod that chekc if it is of a type and then trnasfer sit into a diffent aribale is because i cant do Dictionary<string, Column<?>> columns = new Dictionary<string, Column<?>>();

namespace in_memory_db
{
    public class Column<T> : IColumn
    {
        private T[] _startingCells;
        private List<T> _newCells;
        public T tempCellVal;

        public Column(int startingSize = 0)
        {
            _startingCells = new T[startingSize];
            _newCells = new List<T>();
        }



        public void AddInitialCell(int rowIndex, T val)
        {
            _startingCells[rowIndex] = val;
        }

        public void AddCell<Q>(Q val)
        {
            if (val is T t)
            {
                _newCells.Add(t);
            }
            else
            {
                throw new Exception("Generic type of method was different from the column's type.");
            }
        }




        public Q GetCell<Q>(int index)
        {
            T t;
            if (index < _startingCells.Length)
            {
                t = _startingCells.ElementAt(index);
            }
            else
            {
                t = _newCells.ElementAt(index);
            }
            if (t is Q q)
            {
                return q;
            }
            throw new Exception("Generic type of method was different from the column's type.");
        }

        public void GetCell(int rowIndex, out dynamic val)
        {
            if (rowIndex < _startingCells.Length)
            {
                val = _startingCells.ElementAt(rowIndex);
            }
            val = _newCells.ElementAt(rowIndex);
        }




        public void SetCell<Q>(int index, Q val)
        {
            if (val is T t)
            {
                if (index < _startingCells.Length)
                {
                    _startingCells[index] = t;
                }
                _newCells[index] = t;
            }
            else
            {
                throw new Exception("Generic type of method was different from the column's type.");
            }
        }





        public int GetSize()
        {
            return _startingCells.Length + _newCells.Count;
        }




        public void Swap(int index1, int index2)
        {
            int size = GetSize();
            if (index1 > size || index2 > size)
            {
                throw new ArgumentException();
            }

            T cell1 = GetCell<T>(index1);
            T cell2 = GetCell<T>(index2);
            SetCell(index1, cell2);
            SetCell(index2, cell1);
        }



        public Q GetTempCell<Q>()
        {
            if (tempCellVal is Q q) //should awas be true. this si jsut the way i have to cast it. this si so tha user in SameRowAccesor can use this method on an IColumn, menaing there doesnt have to ab casting to Column.
            {
                return q;
            }
            throw new Exception("Generic type of method was different from the column's type.");
        }

        public void AddTempCellPermanently()
        {
            AddCell(tempCellVal);
        }



        public IColumnWrapper GetColumnWrapper()
        {
            return new ColumnWrapper<T>(this);
        }


        public override bool Equals(object obj)
        {
            if(obj is not Column<T>)
            {
                return false;
            }
            Column<T> otherColumn = (Column<T>)obj;
            if (GetSize() != otherColumn.GetSize() || !_startingCells.SequenceEqual(otherColumn._startingCells) || !_newCells.SequenceEqual(otherColumn._newCells))
            {
                return false;
            }
            return true;
        }


        public override int GetHashCode()
        {
            //todo Needed because you overrode Equals().
            return -1;
        }



        public Type GetColumnType()
        {
            return typeof(T); 
        }
    }
}