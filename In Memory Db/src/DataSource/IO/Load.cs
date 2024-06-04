using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using in_memory_db.DataSource.DB;

namespace in_memory_db.DataSource.IO
{
    /// <summary>
    /// Provides static functions to load a saved database into memory.
    /// </summary>
    public static class Load
    {
        // TODO:
        // update Load and Save to use Box type instead of dynamic to avoid auto-boxing?
        // update Load (and also Save maybe) to be more efficient by taking advantage of buffering with reading direct bytes instead of the types directly
        //      also, maybe read all bytes right away with File.ReadAllBytes()?
        //      see: https://www.jacksondunstan.com/articles/3568
        //      note: if reading bytes then we likely need the delimiter characters or some way of knowing when each value starts. with current implementation of 
        //      reading the direct types, there's really no need for delimiter characters
        // add support for null values
        // update classes to be non static? if reading bytes this will probably make the most sense because will have much more info to keep track of per load/save, so
        // a class is probably the best option

        private static Dictionary<Type, Func<BinaryReader, dynamic>> binaryReaderMethods = new();
        private static FileFormat fileFormat = new();

        static Load()
        {
            binaryReaderMethods[typeof(bool)] = (rdr) => rdr.ReadBoolean();
            binaryReaderMethods[typeof(byte)] = (rdr) => rdr.ReadByte();
            binaryReaderMethods[typeof(sbyte)] = (rdr) => rdr.ReadSByte();
            binaryReaderMethods[typeof(char)] = (rdr) => rdr.ReadChar();
            binaryReaderMethods[typeof(decimal)] = (rdr) => rdr.ReadDecimal();
            binaryReaderMethods[typeof(double)] = (rdr) => rdr.ReadDouble();
            binaryReaderMethods[typeof(float)] = (rdr) => rdr.ReadSingle(); // single == float
            binaryReaderMethods[typeof(int)] = (rdr) => rdr.ReadInt32();
            binaryReaderMethods[typeof(uint)] = (rdr) => rdr.ReadUInt32();
            // skipped nint & nuint types
            binaryReaderMethods[typeof(long)] = (rdr) => rdr.ReadInt64(); // long == Int64
            binaryReaderMethods[typeof(ulong)] = (rdr) => rdr.ReadUInt64();
            binaryReaderMethods[typeof(short)] = (rdr) => rdr.ReadInt16(); // short == Int16
            binaryReaderMethods[typeof(ushort)] = (rdr) => rdr.ReadUInt16();

            binaryReaderMethods[typeof(string)] = (rdr) => rdr.ReadString();
        }

        /// <summary>
        /// Loads a database into memory based on the database state stored at the provided file location.
        /// </summary>
        /// <typeparam name="DB">The database class that the returned database should be an instance of</typeparam>
        /// <typeparam name="TBL">The table class that the tables in the database</typeparam>
        /// <param name="fileUri">The file location to load the database from</param>
        /// <returns>A database object instantiated based on the data saved in the provided file location</returns>
        public static DB FromFile<DB, TBL>(Uri fileUri)
            where DB : IDatabase<ITable>, new()
            where TBL : ITable, new()
        {
            using (var stream = File.Open(fileUri.LocalPath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    DB db = new DB();
                    while (reader.PeekChar() != fileFormat.DB_END_CHAR) //should read a null char to hit eof
                    {
                        // read new table into db
                        KeyValuePair<string, TBL> table = ReadTable<TBL>(reader);
                        db.AddTable(table.Key, table.Value);
                    }

                    return db;
                }
            }
            /*}
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save database to file.\nException: {e.Message}\nStack Trace: {e.StackTrace}");
                return default(DB);  // todo return null and
            }*/
        }

        private static KeyValuePair<string, T> ReadTable<T>(BinaryReader reader)
            where T : ITable, new()
        {
            Dictionary<string, string[]> metadata = ReadTableMetadataFromFile(reader); // todo naming
            EnsureValidMetadata(metadata);

            T table = new T();

            string tableName = metadata[fileFormat.TABLE_NAME_METADATA_KEY][0]; // there's only one value with this metadata item - the name
            string[] columnNames = metadata[fileFormat.COLUMN_NAMES_METADATA_KEY];
            string[] columnTypeStrings = metadata[fileFormat.COLUMN_TYPES_METADATA_KEY]; // todo catch possible error if columnNames.Length != columnTypes.Length?
            Type[] columnTypes = columnTypeStrings.Select(Type.GetType).ToArray();

            CreateColumnsInTable(columnNames, table, columnTypes);

            if (reader.ReadChar() != fileFormat.START_TABLE_VALUES_CHAR)
                throw new Exception("error while parsing table values in file - couldn't find table values start"); // todo better error message

            ReadValuesIntoTable(table, reader, columnNames, columnTypes);

            if (reader.ReadChar() != fileFormat.END_TABLE_VALUES_CHAR)
                throw new Exception("error while parsing table in file - couldn't find table end"); // todo better error message

            return new KeyValuePair<string, T>(tableName, table);
        }

        private static void EnsureValidMetadata(Dictionary<string, string[]> metadata)
        {
            if (!metadata.ContainsKey(fileFormat.TABLE_NAME_METADATA_KEY)
                || !metadata.ContainsKey(fileFormat.COLUMN_NAMES_METADATA_KEY)
                || !metadata.ContainsKey(fileFormat.COLUMN_TYPES_METADATA_KEY))
                throw new Exception("error while parsing metadata items in file - couldn't find all necessary table metadata items"); // todo better error message
        }

        private static void CreateColumnsInTable<T>(string[] columnNames, T table, Type[] columnTypes)
            where T : ITable, new()
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                table.CreateColumn(columnNames[i], columnTypes[i]);
            }
        }

        private static Dictionary<string, string[]> ReadTableMetadataFromFile(BinaryReader reader) // todo naming. this method returns just the table object, obtained by looking at the metadata
        {
            Dictionary<string, string[]> metadata = new();

            while (true)
            {
                var kvp = ReadMetadataItemFromFile(reader);
                metadata.Add(kvp.Key, kvp.Value);
                if (reader.PeekChar() == fileFormat.METADATA_ITEM_DELIMITER)
                    reader.ReadChar();
                else
                    break;
            }

            return metadata;
        }

        private static void ReadValuesIntoTable<T>(T table, BinaryReader reader, string[] columnNames, Type[] columnTypesInOrder)
            where T : ITable, new()
        {
            
            while (true)
            {
                for (int i = 0; i < columnTypesInOrder.Length; i++)
                {
                    string columnName = columnNames[i];
                    var binaryReaderMethod = binaryReaderMethods[columnTypesInOrder[i]];
                    table.AddCell(columnName, binaryReaderMethod.Invoke(reader));
                    if (i < columnTypesInOrder.Length - 1) reader.ReadChar();  // only read delimiter between values, not after last value
                }

                if (reader.PeekChar() == fileFormat.ROW_DELIMITER)
                    reader.ReadChar();
                else
                    break;
            }
        }

        private static KeyValuePair<string, string[]> ReadMetadataItemFromFile(BinaryReader reader)
        {
            string[] allMetadata = reader.ReadString().Split(fileFormat.METADATA_VALUES_START_DELIMITER, fileFormat.METADATA_VALUES_END_DELIMITER);
            string key = allMetadata[0];
            string[] values = allMetadata[1].Split(fileFormat.METADATA_VALUES_SEPARATOR);

            return new KeyValuePair<string, string[]>(key, values);
        }
    }
}