using System;

namespace InMemoryDb
{
    public partial class Funcs
    {
        //todo look at end of where params.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cols">cant be of length 0. also,,d elcare them in theorder you want to crate them in, so that if col% wnats to access col# in its lambda, then col# should be in the arya first so its vlaue na be calcauted for col% to access.</param>
        /// <param name="where">this only gets access to the fields of the row just created.
        /// if you want to do a correlated subquery then do a selct on teh table that clautltes
        /// what you want for the inner selct statmetn of what would normally be acocrelate dusbquery in sql
        /// , and then chain a selec stament onto that which uses the data of the orignal tbale and the result from
        /// the preious query.
        /// 
        /// in order to do this, each Col should eb allowed to speicify a diffenrt tbale that
        /// it wants to get its source column from - ther should be no reason that they should all be based on the
        /// same table - nad you should also allow it to use a table from a preivous (in the chain) query.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Funcs Select(string tableName, ICol[] cols, Func<SameRowAccessor, bool> where = null, string nameOfResultTable = null)
        {
            _ScreenArgs(tableName, cols);
            _currResultRows = new Rows();
            _resultTable = new Table(_currResultRows);
            if (where == null)
            {
                where = row => true;
            }

            Table sourceTable = _database.Contains(tableName) ? _database.Get(tableName) : new Table(_results[tableName]);
            SameRowAccessor sameRowAccessor = new SameRowAccessor(_currResultRows);

            foreach (ICol col in cols)
            {
                col.SetSourceColumnWrapper(sourceTable.GetColumnWrapper(col.SourceColumnName));
                col.SetSameRowAccessor(sameRowAccessor);
                IColumn column = col.GetColumn();
                _resultTable.Add(col.ResultColumnName, column);
            }

            int numOfRows = sourceTable.GetNumOfRows();
            for (int i = 0; i < numOfRows; i++)
            {
                foreach (ICol col in cols)
                {
                    //It's temporary because I'm filtering during the Select rather than wasting space and doing it afterwards.
                    col.TemporarilyAdd(i);
                }
                if (where(sameRowAccessor))
                {
                    foreach (ICol col in cols)
                    {
                        col.PermanentlyAdd();
                    }
                }
            }


            if (nameOfResultTable != null)
            {
                _results[nameOfResultTable] = _currResultRows;
            }
            _lastResult = _currResultRows;
            _currResultRows = null;
            _resultTable = null;
            return this;
        }

        private void _ScreenArgs(string tableName, ICol[] cols)
        {
            if (cols.Length == 0)
            {
                throw new ArgumentException();
            }
            if (!_database.Contains(tableName) && !_results.ContainsKey(tableName))
            {
                throw new ArgumentException();
            }
        }
    }
}
