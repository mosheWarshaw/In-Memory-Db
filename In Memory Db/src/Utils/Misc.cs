namespace InMemoryDb
{
    public class Misc
    {
        /*Note, the check for the string type is because creating a collection
         * of type string? will just be created as type string, and so it won't
         * come up as nullable even though the user would want to be able to set
         * some of the elems as null.*/
        public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null || type == typeof(string);
        public static bool IsNullable<V>(V val) => Nullable.GetUnderlyingType(typeof(V)) != null || typeof(V) == typeof(string);
    }
}
