
/*The reason for the if statements in each method that check if it is of a type and then transfers it
 * into a different variable is because i can't do
 * Dictionary<string, Column<?>> columns = new();
 */

//todo Use the observer design pattern for updating foriegn keys when a pk is changed.

//todo Refactor file, and explain that you use default(T) when you want to use null, but can't because the compiler won't allow it even though you know T is nullable. Also explain that if(null is int?) returns false.

namespace InMemoryDb
{
    public class Column<T> : IColumn
    {
        private T[] _startingCells;
        private List<T> _newCells;
        private T _tempVal;

        private bool _isFk;
        private string _referencedTableName;
        private string _referencedPkName;
        //todo When you add Non-null constraints on columns, change these indexes collections to not necessarily be nullables.
        private int?[] _startingCellsIndexes;
        private List<int?> _newCellsIndexes;

        /// <summary>
        /// Use other constructor if this column's values are foreign keys.
        /// </summary>
        public Column(int startingSize = 0)
        {
            _startingCells = new T[startingSize];
            _newCells = new List<T>();
        }

        /// <summary>
        /// Use if this column is a fk column.
        /// </summary>
        public Column(string referencedTableName, string referencedPkName, int startingSize = 0) : this(startingSize)
        {
            _isFk = true;
            _referencedTableName = referencedTableName;
            _referencedPkName = referencedPkName;
            _startingCellsIndexes = new int?[startingSize];
            _newCellsIndexes = new();
        }


        #region fk methhods
        public bool IsFk()
        {
            return _isFk;
        }


        /*todo All the tables should be read from their files into 
         * memory, but the columns holding fks (ie Foreign KeyS)
         * should be left empty. When all the tables have been read in, then
         * fill in the fk columns one table at a time. Reason: tables could have
         * circular key references. So that is when this function should be called
         * for fk columns.*/
        public void SetIndexes(Database db)
        {
            int? address = null;
            T pk;
            for (int i = 0; i < GetSize(); i++)
            {
                pk = GetCell<T>(i);
                if (pk != null)
                    address = db.Get(_referencedTableName).GetIndexOfNth(_referencedPkName, pk, 1);
                else
                    address = null;
                SetIndex(i, address);
            }
        }

        public void SetIndex(int indexToPutItIn, int? address)
        {

            if (indexToPutItIn < _startingCells.Length)
                _startingCellsIndexes[indexToPutItIn] = address;
            else
            {
                /*Because doing
                 *      List<int> list = new List<int>(5);
                 *      list[0] = 1234567890;
                 * gives you an error for using the brackets
                 * without an elem having already been placed there first.
                 * Normally you wouldn't notice this because you use the list's
                 * Add method.*/
                while (indexToPutItIn >= _newCellsIndexes.Count)
                    _newCellsIndexes.Add(0);
                _newCellsIndexes[indexToPutItIn] = address;
            }
        }

        public int? GetPkIndex(int fkIndex)
        {
            if (!_isFk)
                throw new Exception("This column is not an fk column.");
            if (fkIndex < _startingCellsIndexes.Length)
                return _startingCellsIndexes[fkIndex];
            return _newCellsIndexes.ElementAt(fkIndex);
        }
        #endregion



        #region add cell
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
            //todo IsNullable doesn't work if the type is a sring, because doing Table.Create<string?> will just creataa type of string, and so the null not being of any type will fail the if statement, and will fail the Misc.IsNullable, so this typeof chekcig is eneded. todo add it o all other palces in this file.
            else if (val == null && Misc.IsNullable(val))
                //Because I can't add null directly, I use the default, which in the case of a Nullable it's null.
                _newCells.Add(default(T));
            else
                throw new GenericTypeException();
        }
        #endregion


        #region get cell
        public V GetCell<V>(int index)
        {
            T t;
            if (index < _startingCells.Length)
                t = _startingCells.ElementAt(index);
            else
                t = _newCells.ElementAt(index);
            if (t is V v)
                return v;
            else if (t == null && Misc.IsNullable(t))
                return default(V);
            throw new GenericTypeException();
        }

        //todo use this one within Column.
        //For when you aren't working with an IColumn, and you dont need the if statement to make sure the right type was used. I use a diffenrent method name to ensure that GtCell<V> is never used when ou mean to sue this one, and it oculd happne without you relaizing it becuse thetype of GetCell<V> could be inferrered and you wont get a compiler error for leaving out the <V>.
        public T GetCellT(int index)
        {
            if (index < _startingCells.Length)
                return _startingCells.ElementAt(index);
            return _newCells.ElementAt(index);
        }

        public void GetCell(int rowIndex, out dynamic val)
        {
            if (rowIndex < _startingCells.Length)
                val = _startingCells.ElementAt(rowIndex);
            else
                val = _newCells.ElementAt(rowIndex);
        }
        #endregion



        public void SetCell<V>(int index, V val)
        {
            if (val is T tVal)
            {
                if (index < _startingCells.Length)
                    _startingCells[index] = tVal;
                else
                    _newCells[index] = tVal;
            }
            else if (val == null && Misc.IsNullable(val))
            {
                if (index < _startingCells.Length)
                    _startingCells[index] = default(T);
                else
                    _newCells[index] = default(T);
            }
            else
                throw new GenericTypeException();
        }


        #region temp val
        public void SetTempVal(T val)
        {
            _tempVal = val;
        }

        public V GetTempVal<V>()
        {
            if (_tempVal is V v)
                return v;
            else if (_tempVal == null && Misc.IsNullable(_tempVal))
                return default(V);
            throw new GenericTypeException();
        }

        public void AddTempVal()
        {
            _newCells.Add(_tempVal);
        }
        #endregion



        /// <summary>
        /// This is not the only mechanism to be used when the user wants to delete a row.
        /// In that case, Rows should contain a set of removed elements so that it knows what rows not to write back into
        /// storaage when closing down the db and what rows to not allow retrieval of.
        ///
        /// todo Because of the last part of what I said above, Col should not get a column it can access, rather it should
        /// get a wrapper of some sort that allows it to only access the column it should be allowed to,
        /// and becuase there could be missing indexes the user should get an iterator to go through the rows of the result they
        /// are returned rather than passing in indexes. Each call to the iteratoer should return a
        /// rows wraper that has the index of which row to use (reuse the obejct and just use an UnmodifiableVal (ie a
        /// class holiding an object that was pased in. this passed in object should be mmodifiable, and should be held onto so that
        /// the unmodifiableval can e given to someone who someone who it should be unmodifable for, but in reality you will be able to modify)),
        /// and the enumerator shoud be like
        /// 
        /// public IEnumerable<ATypeOfRowsWrapper> Enumerator()
        /// {
        ///     while(index < size)
        ///     {
        ///         if(removedRows.Contains(index))
        ///             index++;
        ///         else
        ///         {
        ///             modifiableVal.Val = index;
        ///             yeild return aRowsWrapper;
        ///         }
        ///      }
        /// }
        /// 
        /// todo When a user inserts a row, check the set of removed rows to see if there is a gap of a
        /// row that can be filled up.
        /// </summary>
        public void Remove(int index)
        {
            if (index < _startingCells.Length)
                _startingCells[index] = default(T);
            else
                _newCells.RemoveAt(index);
        }


        public int GetSize()
        {
            return _startingCells.Length + _newCells.Count;
        }
        

        public void Swap(int index1, int index2)
        {
            int size = GetSize();
            if (index1 > size || index2 > size)
                throw new ArgumentException();

            T cell1 = GetCell<T>(index1);
            T cell2 = GetCell<T>(index2);
            SetCell(index1, cell2);
            SetCell(index2, cell1);
        }


        public IColumnWrapper GetColumnWrapper()
        {
            return new ColumnWrapper<T>(this);
        }


        public int GetIndexOfNth<U>(U val, int n)
        {
            int counter = 0;
            for (int i = 0; i < GetSize(); i++)
            {
                if (val.Equals(GetCell<T>(i)))
                {
                    counter++;
                    if (counter == n)
                        return i;
                }
            }
            return -1;
        }


        public override bool Equals(object obj)
        {
            if (obj is not Column<T>)
                return false;
            Column<T> otherColumn = (Column<T>)obj;
            if (GetSize() != otherColumn.GetSize() || !_startingCells.SequenceEqual(otherColumn._startingCells) || !_newCells.SequenceEqual(otherColumn._newCells))
                return false;
            return true;
        }


        public override int GetHashCode()
        {
            //todo Needed because you overrode Equals().
            return -1;
        }


        //For adapter only.
        public Type GetColumnType()
        {
            return typeof(T);
        }
    }
}