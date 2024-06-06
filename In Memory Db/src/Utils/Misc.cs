namespace InMemoryDb
{
    public class Misc
    {
        //Official way to check if type is nullable. https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types#how-to-identify-a-nullable-value-type
        public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null || type == typeof(string);
        public static bool IsNullable<V>(V val) => Nullable.GetUnderlyingType(typeof(V)) != null || typeof(V) == typeof(string);
        //Note, the explicit check for the string type is becuase it wont be registeered as Nullable. Look at comments in Column that discuss this.
    }
}
