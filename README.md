A C# library of an in memory database, with the tables being stored and retrieved from self created and managed files, with functions corresponding to SQL commands, and a linear time JOIN.
<br>

This project was done with [@avromi-s](https://github.com/avromi-s) with whom the planning and designing of the entire project was done together, and we then took separate parts (him: DataSource (ie the storing and retrieving of the database). me: the structure of the tables when in memory, and the query commands), and we linked them with an Adapter.

Regarding overall design
------------------------
Arrays contain metadata, so the fewer arrays in a table then the less space database table will take up.
Having the db be a 2d array where each inner array is table row in the table will have a lot of arrays.
A single array for the whole table (and if the table has 7 columns in the table, then you would get the tenth row by taking elems from 69 (7*10 in 0-index-counting) through 76 (69+7)) is not possible in a strongly typed language unless you use an Object as the type of the array, and waste space that way.
So instead, there is a single array per column. The table is a map of column names to the column. To get the second row from the table you would take the second elem from each array.

Regarding usage
---------------
SQL commands are corresponded to function calls, and these calls can be chained together to create long compelx queries. The user, in eahc function, can give a name to the results of the function, so the results can be accessed later in the chain, as if accessing a table in the database.

Current commands
----------------
```Select``` <br>
```Where``` <br>
```OrderBy``` <br>
```JoinOnKeys``` <br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Regarding ```Select```</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;It accepts an array of ```Col```s in which you can define all the things you would want to be able to do in a regular SQL select, such as renaming a column, and using the values of other cells in the row to determine the value, to mention a couple.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Regarding ```JoinOnKeys```</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;JoinOnKeys refers to doing a join on rows on the foriegn key in one relating to the primary key on the other, as opposed to
doing a cartesian product on the tables and filtering based on a condition. My implementation of this method was the reason I
decided to do a database library in the first place. The idea of doing a product (ie relational algebra) in every join seemed
wasteful if you were doing it on keys, ie rows that knew exactly which rows of the other table to be matched up with. I
thought that in a language like C++ where the addresses can be stored as variables, that foreign keys could be addresses of the row in which it wanted to be connected to, and this way joining on keys would be
a linear operation, not a polynomial one. Being that I wanted to do a project in C# at the time, in order to get more experience
using it, I decided to do the project in it rather than in C++ or anther low level language where you could use addresses, and
instead of memory addresses, array/List indexes are used. They aren't used in place of foreign keys, so there is the additional
```n``` space for every column whose values are foreign keys, where ```n``` is the number of rows. When a table is read into memory,
all the tables are read in and then each fk column creates a list of all the corresponding indexes of the pk rows.


Regarding design of specific implementation
-------------------------------------------
The ```Funcs``` class is what the querier uses. It contains all the SQL commands as functions. It utilizes the functionality provided by ```Table```. ```Table``` holds ```Rows```, each ```Rows``` being a collection of ```Column```s. ```Column``` abstracts the data type of the column. The ```IColumn``` interface is used by the other parts because they need to be working on the Column regardless of the type, and as much as can be done regarding the specific cells and their types that needs to be done is on the Column level so that the higher levels don't have to use if-else statements to find the type of the Column, and that boxing doesn't need to be done (note, ```dynamic``` uses System.Object so it would cause boxing).
<br><br>
```Rows``` is a class that is primarily a holder a data (ie the columns), not a provider of functionality. Because it is used in situations where the user of it should have altering-privileges, but not in other scenarios, the members of ```Rows``` are public, and when the user should only have access-privileges then they are given ```Rows``` wrapped in ```RowsWrapper```.
<br>
```Table```, being primarily a provider of functionality, and its only data being ```Rows```, has 2 constructors. One that creates its own ```Rows```, and one that takes one as a parameter and doesn't do defensive copying. The reason for the second is that sometimes the functionality of ```Table``` is wanted, but not without losing the direct access to ```Rows```. But in cases where you wouldn't want ```Rows``` to be directly accessed from ```Table```, ```Rows``` is kept safe by being a private field. Another advantage of the second is that it provides the ability when testing to check 
 directly if ```Table```'s functionality had the proper affect on ```Rows```.
