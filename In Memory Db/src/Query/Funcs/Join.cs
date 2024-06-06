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
            //if(on != null) screenColName(on);
            //if(resultTableName != null) screen table name is untaken in db and _resultRows
            //if(joinType == JoinType.FULL || joinType == JoinType.LEFT) screen table(s) that column type is nullable 
            //_ScreenSelectArgs(leftTableName, leftTableCols);
            //_ScreenSelectArgs(rightTableName, rightTableCols);
            _currResultRows = new Rows();
            _resultTable = new Table(_currResultRows);
            if (where == null)
            {
                where = row => true;
            }

            SameRowAccessor sameRowAccessor = new SameRowAccessor(_currResultRows);

            Table fkTable = _database.Contains(fkTableName) ? _database.Get(fkTableName) : new Table(_results[fkTableName]);
            Table pkTable = _database.Contains(pkTableName) ? _database.Get(pkTableName) : new Table(_results[pkTableName]);


            Table sourceTable = fkTable;
            ICol[] cols = fkCols;
            for (int i = 0; i < 2; i++)
            {
                foreach (ICol col in cols)
                {
                    col.SetSourceColumnWrapper(sourceTable.GetColumnWrapper(col.SourceColumnName));
                    col.SetSameRowAccessor(sameRowAccessor);
                    IColumn column = col.GetColumn();
                    _resultTable.Add(col.ResultColumnName, column);
                }
                sourceTable = pkTable;
                cols = pkCols;
            }


            HashSet<int> indexesOfAddedRight = new();

            /* regarding outer joins for doing oins with keys:
             *    the fkTable row is taken care of in teh
             *   iterations (it is added whether it passes or fails the where()), and the pkTable's rows
             *   are always added to teh set so they can be done later (whether the pkTable is the left
             *   tbale in a left join, or it's a full join).
             * regarding outer joins for doing joins via carteisain product:
             *   the left table is always take care of (added if it passes or fails the where),
             *   and the right is always added to teh set to be done later (this is the case when the join type is
             *   FULL).
             */


            int? pkIndexNullable;
            int pkIndex;

            for (int i = 0; i < fkTable.GetNumOfRows(); i++)
            {
                foreach (ICol col in fkCols)
                    col.TemporarilyAdd(i);
                pkIndexNullable = fkTable.GetPkIndex(i, onFk);
                if (pkIndexNullable != null)
                {
                    pkIndex = pkIndexNullable.Value;
                    foreach (ICol col in fkCols)
                        col.TemporarilyAdd(i);
                    foreach (ICol col in pkCols)
                        col.TemporarilyAdd(pkIndex);
                    if (where(sameRowAccessor))
                    {
                        foreach (ICol col in fkCols)
                            col.PermanentlyAdd();
                        foreach (ICol col in pkCols)
                            col.PermanentlyAdd();

                        if (joinType == JoinType.FULL || joinType == JoinType.RIGHT)
                        {
                            indexesOfAddedRight.Add(pkIndex);
                        }
                    }
                    else if (joinType == JoinType.LEFT)
                    {
                        foreach (ICol col in fkCols)
                            col.PermanentlyAdd();
                        foreach (ICol col in pkCols)
                            col.AddNull();
                    }
                }
                else
                {
                    if (joinType == JoinType.LEFT)
                    {
                        foreach (ICol col in fkCols)
                            col.PermanentlyAdd();
                        foreach (ICol col in pkCols)
                        {
                            col.AddNull();
                        }
                    }
                }
            }


            _EndOfFunc(nameOfResultTable);
            return this;
        }

        //does cartesian product.
        public Funcs JoinWhere(/*...*/)
        {
            //look at paper.
            return this;
        }
    }
}
