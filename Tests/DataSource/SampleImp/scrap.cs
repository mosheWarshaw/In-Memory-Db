using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;

namespace in_memory_db_tests.DataSource.SampleImp
{
    // demo of writing and reading dynamic values to a file with BinaryFormatting:
    internal class SampleDynamicReadAndWrite
    {
        static string path = @"c:\temp\test.dat"; // ****
        static Dictionary<Type, Func<BinaryReader, dynamic>> binReaderMethods = new Dictionary<Type, Func<BinaryReader, dynamic>>();

        [Serializable]
        class Item
        {
            public string strVal = "1";
            public int intVal = 1;

            public override string ToString()
            {
                return $"strVal:{strVal}, intVal:{intVal}";
            }

            public void foo()
            {
                Console.WriteLine("in foo");
            }
        }

        static void Main(string[] args)
        {
            //long serializableTime = TestSerializableRowsStorage();
            //long serializableFileSize = new FileInfo(path).Length / 1000;
            long customTime = TestCustomRowsStorage();
            long customFileSize = new FileInfo(path).Length / 1000;
            Console.WriteLine($"custom time: {customTime}");
            //Console.WriteLine($"custom is : {serializableTime - customTime}ms faster ({(((serializableTime - customTime) / (double)customTime) * 100)}%)");
            //Console.WriteLine($"serializable file size: {serializableFileSize}kb");
            Console.WriteLine($"custom file size: {customFileSize}kb");
            //Console.WriteLine($"custom is : {serializableFileSize - customFileSize}kb smaller ({(((serializableFileSize - customFileSize) / (double)customFileSize) * 100)}%)");
            Console.ReadKey();
        }

        static long TestSerializableRowsStorage()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            using (var stream = File.Open(path, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(stream, GetSampleRows());
            }

            using (var stream = File.Open(path, FileMode.Open))
            {
                var bf = new BinaryFormatter();
                IEnumerable<dynamic[]> readRows = (IEnumerable<dynamic[]>)bf.Deserialize(stream);
                foreach (dynamic[] row in readRows)
                {
                    foreach (dynamic val in row)
                    {
                        Console.WriteLine($"type: {val.GetType()}, value:{val}");
                    }
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            return elapsedMs;
        }

        static long TestCustomRowsStorage()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            binReaderMethods[typeof(string)] = (rdr) => rdr.ReadString();
            binReaderMethods[typeof(int)] = (rdr) => rdr.ReadInt32();
            Type[] types = { typeof(string), typeof(int) };
            WriteRowsToFile(GetSampleRows());
            IEnumerable<dynamic[]> readRows = ReadRowsFromFile(types);
            foreach (dynamic[] row in readRows)
            {
                foreach (dynamic val in row)
                {
                    Console.WriteLine($"type: {val.GetType()}, value:{val}");
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            return elapsedMs;
        }

        static void WriteRowsToFile(IEnumerable<dynamic[]> rows)
        {
            using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (dynamic[] row in rows)
                    {
                        foreach (dynamic val in row)
                        {
                            writer.Write(val);
                        }

                        writer.Write('\n');
                    }

                    writer.Write('\0');
                }
            }
        }

        static IEnumerable<dynamic[]> ReadRowsFromFile(Type[] types)
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    while (reader.PeekChar() != '\0') //should read a null char to hit eof
                    {
                        dynamic[] row = new dynamic[types.Length];
                        for (int i = 0; i < types.Length; i++)
                        {
                            row[i] = binReaderMethods[types[i]].Invoke(reader);
                        }

                        yield return row;
                        reader.ReadChar(); // should read a new line char to hit end of row
                    }
                }
            }
        }

        static List<dynamic[]> GetSampleRows()
        {
            List<dynamic[]> rows = new List<dynamic[]>();
            for (int i = 0; i < 100000; i++)
            {
                rows.Add(new dynamic[] { i.ToString(), i });
            }

            return rows;
        }
    }
}