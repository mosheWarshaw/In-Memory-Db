using System;
using System.Data.Common;

namespace InMemoryDb.src.Query.Col
{
    //<Source, Result> aka datatype of the sourceColumn, and datatype of the resultColumn (aka the datatype of what TheAction "returns" (aka the type of box it stores the value in)).
    public class Col<S, R> : ICol
    {
        public string SourceColumnName { get; }
        public string ResultColumnName { get; }
        private readonly Func<S, SameRowAccessor, R> _theFunc;
        private Func<SameRowAccessor, R> _sourcelessFunc;

        public Column<R> column = new Column<R>();

        private ColumnWrapper<S> _sourceColumnWrapper;
        private SameRowAccessor _sameRowAccessor;


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
            if (SourceColumnName != null)
            {
                S s = _sourceColumnWrapper.GetCell(index);
                if (_theFunc != null)
                {
                    column.SetTempVal(_theFunc(s, _sameRowAccessor));
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
    }
}
