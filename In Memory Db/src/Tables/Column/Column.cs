
/*The reason for the if statements in each ethod that chekc if it is of a type and then transfers it
 * into a different variable is because i can't do
 * Dictionary<string, Column<?>> columns = new();
 */

namespace InMemoryDb
{
    public class Column<T> : IColumn
    {
        private T[] _startingCells;
        private List<T> _newCells;
        private T _tempVal;

        public Column(int startingSize = 0)
        {
            _startingCells = new T[startingSize];
            _newCells = new List<T>();
        }



        public void AddInitialCell(int rowIndex, T val)
        {
            _startingCells[rowIndex] = val;
        }

        public void AddCell<V>(V val)
        {
            if (val is T tVal)
            {
                _newCells.Add(tVal);
            }
            else
            {
                throw new Exception("Generic type of method was different from the column's type.");
            }
        }




        public V GetCell<V>(int index)
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
            if (t is V v)
            {
                return v;
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




        public void SetCell<V>(int index, V val)
        {
            if (val is T tVal)
            {
                if (index < _startingCells.Length)
                {
                    _startingCells[index] = tVal;
                }
                _newCells[index] = tVal;
            }
            else
            {
                throw new Exception("Generic type of method was different from the column's type.");
            }
        }

        public void SetTempVal(T val)
        {
            _tempVal = val;
        }

        public V GetTempVal<V>()
        {
            if (_tempVal is V v)
                return v;
            throw new Exception("Generic is wrong type.");
        }

        public void AddTempVal()
        {
            _newCells.Add(_tempVal);
        }


        /// <summary>
        /// This is not the only mechanism to be used when the user wants to delete a row.
        /// In that case, Rows should contain a set of removed elements so that it knows what rows not to write back into
        /// storaage when closing down the db and what rows to not allow retrieval of.
        /// todo because of the last part of what i said above, Col should not get a column it can caccess, rather it should
        /// get a wpper of ome sort that allows itto only access the column it should be allowed to,
        /// and becuase there could be missing indexes the uer should get an iterator to go through the rows f the result they
        /// are returned rathe than passing in indexes. each call to the iteratoer should return a
        /// rows wraper that has the index of which row to use (reuse teh obejct and jsut an UnmodifiableVal), and the enumerator shoud
        /// be like
        /// public IEnumerable<ARowsWrapper> Enumerator()
        /// {
        ///     while(index < size)
        ///     {
        ///     
        ///         if(removedRows.Contains(index))
        ///             index++;
        ///         else
        ///         {
        ///             modifiableVal.Val = index;
        ///             yeild return aRowsWrapper;
        ///         }
        /// }
        /// 
        /// note, when a user inserts a row, check the set of removed rows to see if there is a gap of a row that can be filled up.
        /// </summary>
        public void Remove(int index)
        {
            if (index < _startingCells.Length)
            {
                _startingCells[index] = default(T);
            }
            else
            {
                _newCells.RemoveAt(index);
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