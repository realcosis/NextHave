using System.Reflection;

namespace NextHave.BL.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSimpleType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsSimpleType(type.GetGenericArguments()[0]); // nullable type, check if the nested type is simple.

            var typeInfo = type.GetTypeInfo();

            return typeInfo.IsPrimitive || typeInfo.IsEnum || type.Equals(typeof(decimal)) || type.Equals(typeof(string)) ||
                   type.Equals(typeof(DateTime)) || type.Equals(typeof(Guid)) || type.Equals(typeof(DateTimeOffset)) ||
                   type.Equals(typeof(TimeSpan)) || type.Equals(typeof(Uri));
        }
    }
}