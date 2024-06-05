namespace InMemoryDb
{

    //todo in middle

    public partial class Funcs
    {
        public enum JoinType { INNER, LEFT, FULL };
        //<leftRow, rightRow, _> where
        //if there is no "on" arg, then you do cartiesian product. on is for when joining based on keys, but for joins such as      join table1 and table2 on table1.col > tabl2.col + 200       you would use teh where Func to tell me if i should include the row.
        //result tbale name can never be null.
        public Funcs Join(string leftTableName, ICol[] leftTableCols, string rightTableName, ICol[] rightTableCols, string on = null, Func<SameRowAccessor, bool> where = null, JoinType joinType = JoinType.INNER, string nameOfResultTable = null)
        {
            //if(on != null) screenColName(on);
            //if(resultTableName != null) screen table name is untaken in db and _resultRows
            _ScreenSelectArgs(leftTableName, leftTableCols);
            _ScreenSelectArgs(rightTableName, rightTableCols);
            _currResultRows = new Rows();
            _resultTable = new Table(_currResultRows);
            if (where == null)
            {
                where = row => true;
            }

            SameRowAccessor sameRowAccessor = new SameRowAccessor(_currResultRows);

            Table leftSourceTable = _database.Contains(leftTableName) ? _database.Get(leftTableName) : new Table(_results[leftTableName]);
            Table rightSourceTable = _database.Contains(rightTableName) ? _database.Get(rightTableName) : new Table(_results[rightTableName]);

            Table sourceTable = leftSourceTable;

            foreach (ICol[] cols in (new ICol[][] { leftTableCols, rightTableCols}) ) {

                //todo when  you finish, you should see if you can remove the duplication with the select function.
                foreach (ICol col in cols)
                {
                    col.SetSourceColumnWrapper(sourceTable.GetColumnWrapper(col.SourceColumnName));
                    col.SetSameRowAccessor(sameRowAccessor);
                    IColumn column = col.GetColumn();
                    _resultTable.Add(col.ResultColumnName, column);
                }

                sourceTable = rightSourceTable;

            }



            if (on != null)
            {
                /*Table fkTable = ;
                Table pkTable = ;
                for(int i = 0; i < )*/
            }
            else
            {
                
            }


            _EndOfFunc(nameOfResultTable);
            return this;
        }
    }
}
