﻿namespace InMemoryDb
{
    public partial class Funcs
    {
        /// <summary>
        /// <para>
        ///     <c>cols</c>:
        ///     Can't be of length 0.
        ///     Also, declare them in the order you want them to be filled,
        ///     so that if col% wants to access col# in its lambda, then col#
        ///     should be in the arrray before col#.
        /// </para>
        /// <para>
        ///     <c>where</c>:
        ///     This only gets access to the fields of the row just created.
        ///     If you want to do a correlated subquery then do 2 chained Selects, and
        ///     do that outer select after the inner select (remember, a tablename for a Select
        ///     can be the anme you gave to the result of a previous query).
        /// </para>
        /// <para>
        ///     <c>nameOfResultTable</c>:
        ///     Cannot be set to null, else it will be ignored.
        /// </para>
        /// <para>
        ///     <c>amountToTake</c>:
        ///     If set, the function will take the first n amount of rows that pass the where function, where n = amountToTake.
        /// </para>
        /// </summary>
        public Funcs Select(string tableName, ICol[] cols, Func<SameRowAccessor, bool> where = null, string nameOfResultTable = null, int? amountToTake = null)
        {
            #region setup
            _ScreenCols(cols);
            _ScreenExistingTableNames(tableName);
            if (nameOfResultTable != null)
                _ScreenNewTableNames(nameOfResultTable);
            _SetUpFunc(ref where);
            SameRowAccessor sameRowAccessor = new SameRowAccessor(_currResultRows);
            ColsSetUp(tableName, out Table sourceTable, cols, sameRowAccessor);
            int numOfAddedRows = 0;
            bool amountTaken = false;
            #endregion



            int numOfRows = sourceTable.GetNumOfRows();
            for (int i = 0; i < numOfRows && !amountTaken; i++)
            {
                foreach (ICol col in cols)
                    col.TemporarilyAdd(i);
                if (where(sameRowAccessor))
                {
                    foreach (ICol col in cols)
                        col.PermanentlyAdd();

                    if(amountToTake != null)
                    {
                        numOfAddedRows++;
                        if (numOfAddedRows == amountToTake)
                            amountTaken = true;
                    }
                }
            }

            

            _EndOfFunc(nameOfResultTable);
            return this;
        }
    }
}
