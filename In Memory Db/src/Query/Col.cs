using System;

namespace InMemoryDb
{
    //<Source, Result> aka datatype of the sourceColumn, and datatype of the resultColumn (aka the datatype of what TheAction "returns" (aka the type of box it stores the value in)).
    public class Col<S, R> : ICol
    {
        public string SourceColumnName { get; }
        public string ResultColumnName { get; }
        private readonly Func<S, SameRowAccessor, R> _theFunc;

        public Column<R> column = new Column<R>();

        private ColumnWrapper<S> _sourceColumnWrapper;


        private SameRowAccessor _sameRowAccessor;
        private Func<SameRowAccessor, R> _sourcelessFunc;


        /*Note, the builder pattern is not right for here because I want the user to create a Col under one of these conditions,
         * and tehre should eb a comiler error if they don't (rather than a runtime error when checking if a valid build
         * can be made, from within the build() method of a would-be builder).*/
        public Col(string sourceColumnName, string resultColumnName = null)
        {
            SourceColumnName = sourceColumnName;
            ResultColumnName = resultColumnName ?? sourceColumnName;
        }

        public Col(string sourceColumnName, string customColumnName, Func<S, SameRowAccessor, R> func) : this(sourceColumnName, customColumnName)
        {
            _theFunc = func;
        }

        public Col(string customColumnName, Func<SameRowAccessor, R> sourcelessFunc)
        {
            SourceColumnName = null;
            ResultColumnName = customColumnName;
            _sourcelessFunc = sourcelessFunc;
        }

        public IColumn GetColumn()
        {
            return column;
        }


        public void SetSameRowAccessor(SameRowAccessor sameRowAccessor)
        {
            _sameRowAccessor = sameRowAccessor;
        }

        public void SetSourceColumnWrapper(IColumnWrapper iColumnWrapper)
        {
            _sourceColumnWrapper = (ColumnWrapper<S>)iColumnWrapper;
        }

        public void TemporarilyAdd(int index)
        {
            /*go thorugh sourceColumnWrapper and put the values through teh Func and store
             * the result in the colum fiedl. then teh Funcs can call he method to get teh
             * column proeprty and add it to the _resultRows. the change of all this is becuase
             * i have a post it card where i show tht teh parts of teh code that have to dela
             * with eh speicific type are on teh opoite ends, and all teh code in between has
             * to try not to do any aitoboxing or casting, if that's even possile regardles of
             * how ugly it becomes.*/
            //also, lok atht eh pseudo code for the updated Select function.


            if (SourceColumnName != null)
            {
                S s = _sourceColumnWrapper.GetCell(index);
                if (_theFunc != null)
                {
                    column.tempCellVal = _theFunc(s, _sameRowAccessor);
                }
                else
                {
                    if (s is R r) //will always be true. is just for converting s to r, becuase ocmpiler doens tknow in this case that S and R are te same.
                    {
                        column.tempCellVal = r;
                    }
                }
            }
            else
            {
                column.tempCellVal = _sourcelessFunc(_sameRowAccessor);
            }
        }

        public void PermanentlyAdd()
        {
            column.AddTempCellPermanently();
        }
    }
}
