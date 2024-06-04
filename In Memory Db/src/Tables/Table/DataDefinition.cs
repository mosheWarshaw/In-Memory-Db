using System;

namespace in_memory_db
{
    //the idea of this rows thing is that instead of usig composition ie having Table have a Rows object (although ti sitll has ne in this case, but), rather than inehiritng is becuase with compositon then Table would need to create delgate method sso that users who ant to use Table ad call the methods of rows hten Tabewill have to pass on the request, but with iheritance then the user who is suing Table can access all of rows methods normally. intead of Table using Rows and plugging in teh dta needed for it, rather Table is in another way using teh methods of Rows but havig Rows use teh data Table wants it to.
    public partial class Table
    {
        protected Rows _rows { get; }

        //you sue the no-args construcotr when you dont want to hold onto the Rows object outside of the Table object you are creating.

        /*Way to have a Table no-args constructor. This way is necessary becuase you need to be able
         * to pass in a Rows object to te base constructor.*/
        public Table() : this(new Rows())
        {
        }

        //this was changed tp not call tehbase construotr, ebcause hat was a msitake. if the user is appsing in a Rwos obejct then you dont need to ceat new one, becuase you' are overriding its _row propprey and it will use this one (this detail was added).
        //Look at RowsWrapper for explanation of Rows being like this, and why we're not doing defensive copying.
        public Table(Rows rows)
        {
            //Deliberately not doing defensive copying.
            _rows = rows;
        }


        public void Create<T>(string columnName, int startingSize = 0)
        {
            if (_rows.Contains(columnName))
            {
                throw new ArgumentException();
            }

            _rows.columns[columnName] = new Column<T>(startingSize);
        }

        public void Add(string columnName, IColumn column)
        {
            if (_rows.Contains(columnName))
            {
                throw new ArgumentException();
            }

            _rows.columns[columnName] = column;
        }
    }
}