A C# library of an in memory database, with the tables being stored and retrieved from self created and managed files.
<br>

This project was done with Avromi Schneierson ([github](https://github.com/avrohom-schneierson), [linkedin](https://www.linkedin.com/in/avrohomschneierson)), with whom the planning and designing of the entire project was done together, and we then took separate parts (him: DataSource (ie the storing and retrieving of the database). me: the structure of the tables when in memory, and the query commands), and we linked them with an Adapter.

Regarding overall design
------------------------
Arrays contain metadata, so the fewer arrays in db table then the less space db table will take up.
Having the db be a 2d array where each inner array is db row in the table will have db lot of arrays, so this isn't good.
A single array for the whole table (and if the table has 7 columns in the table, then you would get the tenth row by taking elems from 69 (7*10 in 0-index-counting) through 76 (69+7)) is not possible in db strongly typed language unless you use an Object as the type of the array, but using objects takes up more space than using primitives, so this isn't good.
So instead, there is db single array per column. The table is map of column names to the column. To get the second row from the table you would take the second elem from each array.


Regarding design of specific implementation
-------------------------------------------
The ```Funcs``` class is what the querier uses. It contains all the SQL commands as funtions. It utilizes the functionality provided by Table. Table holds Rows, each ```Rows``` being a collection of ```Column```s.
```Funcs``` takes a ```Col``` for each column that should be created in the returned results.

Interfaces such as ```ICol``` and ```IColumn``` are used because a map of column-name to column can't be
```Dictionary<string, Column<?>>```
it has to be
```Dictionary<string, IColumn>```
with no type being specified.
Because ```Rows``` and ```Table``` and ```Funcs``` all have to work on different columns, and don't want to interact with it by setting or getting cells with an ```if-else``` statement trying to identify the column type, or with it using boxing (dynamic uses boxing), because of the time it would take to create new objects to wrap the primitives. So as much work that involves the specific type of the cell is on the level of the ```Column``` with the classes using it just telling it what to do. Eg: ```Col``` creates its own ```Columnn``` rather than ```Funcs``` creating it. ```Column``` does its own ```Swap()```ping rather than ```Table``` getting each cell and telling ```Column``` what to set each value (both of these were old ways of handling it).

```Rows``` is a class that is primarily a holder a data, not a provider of functionality, ie the columns. Because it is used in situations where the user of it should have altering-privileges, but not in other scenarios, the members of Rows are public, and when the user should only have access-privileges then they are given ```Rows``` wrapped in ```RowsWrapper```.
```Table```, being primarily a provider of functionality, and its only data being Rows, has 2 constructors. One that creates its own ```Rows```, and one that takes one as a parameter and doesn't do defensive copying. The reason for the second is that sometimes the functionality of ```Table``` is wanted, but not without losing the direct access to ```Rows```. But in cases where you wouldn't want Rows to be directly accessed from Table, ```Rows``` is a private field.
This also provides an advantage for testing, when you can check directly if ```Table```'s functionality had the proper affect on ```Rows```.

Regarding usage
---------------
SQL commands are corresponded to function calls that can be chained upon each other, and in each function you pass in an object containing and managing the creation of a returned column (in the case of data-retrieval queries) in which you have the options as you normally would in a SQL query (ie (speaking of the Select because it is the one with the most features:) naming the column of the table that the values should be taken from, setting the name of the returned column to be different from the original, having a lambda alter the data of the column based off other values in the row or in any way you wish as is presented with the functionality accompanying a c# method, having the column not be based off any value of the table, and being able to assign a name to the results of the query so it can be referenced later in the chain).

Current commands
----------------
```Select``` <br>
```Where``` <br>
```OrderBy``` <br>
```JoinOnKeys``` <br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Regarding ```JoinOnKeys```</b>

