using System.Security.Cryptography.X509Certificates;

namespace InMemoryDb
{
    public class SameRowAccessor
    {
        private readonly Rows _rows;
        public SameRowAccessor(Rows rows)
        {
            _rows = rows;
        }

        public T GetCell<T>(string otherColumnsName)
        {
            return _rows.columns[otherColumnsName].GetTempCell<T>();
        }
    }
}
