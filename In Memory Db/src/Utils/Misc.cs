namespace InMemoryDb
{
    public class Misc
    {
        //Note, the check for the string type is becuase it wont be registeered as Nullable. Look at comments in Column that discuss this.
        public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null || type == typeof(string);
        public static bool IsNullable<V>(V val) => Nullable.GetUnderlyingType(typeof(V)) != null || typeof(V) == typeof(string);
    }
}
