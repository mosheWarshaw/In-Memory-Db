namespace InMemoryDb
{
    //<Source, Result> aka datatype of the sourceColumn, and datatype of the column that will be returned.
    public class Col<S, R> : ICol
    {
        public Column<R> column = new Column<R>();

        private ColumnWrapper<S> _sourceColumnWrapper;
        private SameRowAccessor _sameRowAccessor;



        #region construction
        public string SourceColumnName { get; }
        public string ResultColumnName { get; }
        private Func<S, SameRowAccessor, R> _sourceFunc { get; }
        private Func<SameRowAccessor, R> _sourcelessFunc { get; }

        private Col(string sourceColName, string resultColumnName, Func<S, SameRowAccessor, R> sourceFunc, Func<SameRowAccessor, R> sourcelessFunc)
        {
            SourceColumnName = sourceColName;
            ResultColumnName = resultColumnName ?? sourceColName;
            _sourceFunc = sourceFunc;
            _sourcelessFunc = sourcelessFunc;
        }

        /// <summary>
        /// If there's not a source column, then there needs to be a sourceless func.
        /// Having a sourcefunc and a sourceless func would not make sense, and therefore would yild a thrown exception.
        /// </summary>
        public class Builder
        {
            private string _sourceColumnName;
            private string _resultColumnName;
            private Func<S, SameRowAccessor, R> _sourceFunc;
            private Func<SameRowAccessor, R> _sourcelessFunc;

            #region setters
            public Builder SourceColumnName(string name)
            {
                _sourceColumnName = name;
                return this;
            }

            public Builder ResultColumnName(string name)
            {
                _resultColumnName = name;
                return this;
            }

            /// <summary>
            /// Function to use on each cell of this column, in the case where the column
            /// is based off the cells of an existing table.
            /// </summary>
            public Builder SourceFunc(Func<S, SameRowAccessor, R> func)
            {
                _sourceFunc = func;
                return this;
            }

            /// <summary>
            /// Function to use on each cell of this column, in the case where the column
            /// is not based off the cells of an existing table.
            /// </summary>
            public Builder SourcelessFunc(Func<SameRowAccessor, R> func)
            {
                _sourcelessFunc = func;
                return this;
            }
            #endregion

            public Col<S, R> Build()
            {
                if (VerifyFoundation())
                    return new Col<S, R>(_sourceColumnName, _resultColumnName, _sourceFunc, _sourcelessFunc);
                throw new ImproperFoundationException();
            }

            //Ensure the user has called the right builder methods.
            private bool VerifyFoundation()
            {
                return
                    (_sourceFunc == null || _sourcelessFunc == null)
                    &&
                    (_sourceColumnName == null ^ _sourcelessFunc == null);
            }
        }
        #endregion


        //This is the way it has to be done because Join can't call defualt becuase it doesn't have the R generic type, and it can't just add a null literal because this will throw a compiler error because it doesn't know that the generic type is a Nullable.
        /// <summary>
        /// This will only work if the type is nullable, else the deault type will be something else.
        /// </summary>
        public void AddNull()
        {
            column.AddCell(default(R));
        }


        public IColumn GetColumn()
        {
            return column;
        }


        public int? GetPkIndex(int fkIndex)
        {
            return column.GetPkIndex(fkIndex);
        }



        #region setters
        public void SetSameRowAccessor(SameRowAccessor sameRowAccessor)
        {
            _sameRowAccessor = sameRowAccessor;
        }

        public void SetSourceColumnWrapper(IColumnWrapper iColumnWrapper)
        {
            _sourceColumnWrapper = (ColumnWrapper<S>)iColumnWrapper;
        }
        #endregion



        #region add
        public void TemporarilyAdd(int index)
        {
            if (SourceColumnName != null)
            {
                S s = _sourceColumnWrapper.GetCell(index);
                if (_sourceFunc != null)
                {
                    column.SetTempVal(_sourceFunc(s, _sameRowAccessor));
                }
                else
                {
                    if (s is R r)
                    {
                        column.SetTempVal(r);
                    }
                }
            }
            else
            {
                column.SetTempVal(_sourcelessFunc(_sameRowAccessor));
            }
        }

        public void PermanentlyAdd()
        {
            column.AddTempVal();
        }
        #endregion
    }
}
