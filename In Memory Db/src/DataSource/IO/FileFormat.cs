using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDb.DataSource.IO
{
    /// <summary>
    /// This class provides fields with the various characters used to separate the different parts of the database when stored in the file.
    /// </summary>
    internal class FileFormat
    {
        // This class needs to stay immutable because it makes logical sense and also Save and Load classes will maintain only 1 static
        // reference to it, so nothing should be able to change with those references

        #region constants

        #region symbols

        #region general
        internal readonly char START_TABLE_VALUES_CHAR;
        internal readonly char END_TABLE_VALUES_CHAR;
        internal readonly char ROW_DELIMITER;
        internal readonly char VAL_DELIMITER; // todo what if value contains this character in a string, same issue with row_delimiter
        internal readonly char DB_END_CHAR;

        #endregion

        #region metadata

        internal readonly char METADATA_VALUES_START_DELIMITER;
        internal readonly char METADATA_VALUES_END_DELIMITER;
        internal readonly char METADATA_ITEM_DELIMITER;
        internal readonly char METADATA_VALUES_SEPARATOR;

        internal readonly string TABLE_NAME_METADATA_KEY;
        internal readonly string COLUMN_TYPES_METADATA_KEY;
        internal readonly string COLUMN_NAMES_METADATA_KEY;

        #endregion

        #endregion

        #endregion

        #region structure

        /*private readonly List<string> _order;
        internal IEnumerable<string> Order => _order.AsReadOnly();*/

        #endregion

        public FileFormat()
        {
            // NOTE: some of these characters can NOT match each other as it will cause issues during loading
            // this is most important for the value delimiter and end of row or table symbols, as we need
            // to be able to differentiate between those to know when it is the row/table ending vs just another value.
            // best to keep all values different
            START_TABLE_VALUES_CHAR = '\n';
            END_TABLE_VALUES_CHAR = '+';
            ROW_DELIMITER = '\n';
            VAL_DELIMITER = '_';
            DB_END_CHAR = '\0';

            METADATA_VALUES_START_DELIMITER = '[';
            METADATA_VALUES_END_DELIMITER = ']';
            METADATA_ITEM_DELIMITER = '&';
            METADATA_VALUES_SEPARATOR = VAL_DELIMITER;

            TABLE_NAME_METADATA_KEY = "TABLE_NAME";
            COLUMN_TYPES_METADATA_KEY = "COLUMN_TYPES";
            COLUMN_NAMES_METADATA_KEY = "COLUMN_NAMES";
        }
    }
}