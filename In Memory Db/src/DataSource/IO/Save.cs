using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoryDb.DataSource.DB;

namespace InMemoryDb.DataSource.IO
{
    /// <summary>
    /// Provides static functions to save a database in its current state to a file. The database can then be loaded back into memory at any later point with the
    /// Load class.
    /// </summary>
    public static class Save
    {
        private static FileFormat fileFormat = new FileFormat();

        /// <summary>
        /// Saves the provided database's state to the file location indicated by the fileUri.
        /// </summary>
        /// <param name="fileUri">The file location of where to save the database</param>
        /// <param name="database">The database to save</param>
        /// <returns>true if the file was successfully saved, else false</returns>
        public static bool ToFile(Uri fileUri, IDatabase<ITable> database)
        {
            try
            {
                Dictionary<string, ITable> tables = database.GetTables();

                using (var stream = File.Open(fileUri.LocalPath, FileMode.Create)) // must use fileUri.LocalPath, not absolute
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        foreach (var table in tables)
                        {
                            AppendTableToFile(table.Key, table.Value, writer);
                        }

                        writer.Write(fileFormat.DB_END_CHAR);

                        writer.Flush();
                        writer.Close();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save database to file.\nException: {e.Message}\nStack Trace: {e.StackTrace}");
                return false;
            }
        }

        private static void AppendTableToFile(string tableName, ITable table, BinaryWriter writer) // todo use serialization instead? will it be better performance because no parsing needed?
        {
            WriteTableMetadataToFile(tableName, table, writer);
            writer.Write(fileFormat.START_TABLE_VALUES_CHAR);

            using (var rowEnumerator = table.GetRows().GetEnumerator())  // we use manual enumeration so that we can know before we receive the last value
            {
                rowEnumerator.MoveNext();
                while (true)
                {
                    using (var valEnumerator = rowEnumerator.Current.GetEnumerator())  // we use manual enumeration so that we can know before we receive the last value
                    { 
                        valEnumerator.MoveNext();
                        while (true)
                        {
                            writer.Write(valEnumerator.Current);
                            if (valEnumerator.MoveNext())
                                writer.Write(fileFormat.VAL_DELIMITER);  // only write delimiter between values, not after last value
                            else
                                break;
                        }                     
                    }
                    

                    if (rowEnumerator.MoveNext())
                        writer.Write(fileFormat.ROW_DELIMITER);  // only write delimiter between values, not after last value
                    else
                        break;
                }
            }

            writer.Write(fileFormat.END_TABLE_VALUES_CHAR);
        }

        private static void WriteTableMetadataToFile(string tableName, ITable table, BinaryWriter writer)
        {
            //writer.Write(fileFormat.METADATA_ITEM_DELIMITER);
            WriteMetadataItemToFile(fileFormat.TABLE_NAME_METADATA_KEY, new[] { tableName }, writer);

            writer.Write(fileFormat.METADATA_ITEM_DELIMITER);
            IEnumerable<string> columnNames = table.ColumnValueTypes.Select(tp => tp.Key.ToString());
            WriteMetadataItemToFile(fileFormat.COLUMN_NAMES_METADATA_KEY, columnNames, writer);

            writer.Write(fileFormat.METADATA_ITEM_DELIMITER);
            IEnumerable<string> columnValueTypes = table.ColumnValueTypes.Select(tp => tp.Value.ToString());
            WriteMetadataItemToFile(fileFormat.COLUMN_TYPES_METADATA_KEY, columnValueTypes, writer);
        }

        private static void WriteMetadataItemToFile(string metadataKey, IEnumerable<string> values, BinaryWriter writer)
        {
            writer.Write(metadataKey + fileFormat.METADATA_VALUES_START_DELIMITER + string.Join(fileFormat.METADATA_VALUES_SEPARATOR.ToString(), values) + fileFormat.METADATA_VALUES_END_DELIMITER);
        }
    }
}