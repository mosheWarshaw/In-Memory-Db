namespace InMemoryDb
{
    public partial class Funcs
    {
        //left join differs from an inner join in that leftTable's rows are added wih null values for rightTables columns, when the whee function returns false.
        public enum JoinType { INNER, LEFT, RIGHT, FULL };
        //result tbale name can never be null.
        //fkTable is left, and pktable is right. noe this for choosing th JoinType
        public Funcs JoinOnKeys(string fkTableName, ICol[] fkCols, string onFk, string pkTableName, ICol[] pkCols, string onPk, Func<SameRowAccessor, bool> where = null, JoinType joinType = JoinType.INNER, string nameOfResultTable = null)
        {
            #region set up
            //if(joinType is outer) screen table(s) ensuring column type is nullable.
            _ScreenCols(fkCols, pkCols);
            _ScreenExistingColumnNames(_database, [fkTableName, onFk], [pkTableName, onPk]);
            _ScreenExistingTableNames(fkTableName, pkTableName);
            if (nameOfResultTable != null)
                _ScreenNewTableNames(nameOfResultTable);
            _SetUpFunc(ref where);

            SameRowAccessor sameRowAccessor = new SameRowAccessor(_currResultRows);
            ColsSetUp(fkTableName, out Table fkTable, fkCols, sameRowAccessor);
            ColsSetUp(pkTableName, out Table pkTable, pkCols, sameRowAccessor);

            HashSet<int> indexesOfAddedRight = new();
            int? pkIndex;
            #endregion


            for (int i = 0; i < fkTable.GetNumOfRows(); i++)
            {
                foreach (ICol col in fkCols)
                    col.TemporarilyAdd(i);
                pkIndex = fkTable.GetPkIndex(i, onFk);
                if (pkIndex != null)
                {
                    _TemporarilyAdd(fkCols, i);
                    _TemporarilyAdd(pkCols, pkIndex.Value);
                    if (where(sameRowAccessor))
                    {
                        _PermanentlyAdd(fkCols, pkCols);
                        
                        if (joinType == JoinType.FULL || joinType == JoinType.RIGHT)
                            indexesOfAddedRight.Add(pkIndex.Value);
                    }
                    else if (joinType == JoinType.LEFT)
                        _JoinLeft(fkCols, pkCols);
                }
                else if (joinType == JoinType.LEFT)
                    _JoinLeft(fkCols, pkCols);
            }


            _EndOfFunc(nameOfResultTable);
            return this;
        }







        private void _TemporarilyAdd(ICol[] cols, int i)
        {
            foreach (ICol col in cols)
                col.TemporarilyAdd(i);
        }

        private void _PermanentlyAdd(params ICol[][] cols)
        {
            foreach (ICol[] colArr in cols)
            {
                foreach (ICol col in colArr)
                    col.PermanentlyAdd();
            }
        }

        private void _JoinLeft(ICol[] leftCols, ICol[] rightCols)
        {
            foreach (ICol col in leftCols)
                col.PermanentlyAdd();
            foreach (ICol col in rightCols)
                col.AddNull();
        }






        //does cartesian product.
        public Funcs JoinWhere(/*...*/)
        {
            //look at paper.
            return this;
        }
    }
}
