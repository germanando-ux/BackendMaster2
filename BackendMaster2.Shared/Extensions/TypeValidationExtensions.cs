using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Extensions;

public static class TypeValidationExtensions
{
    // La magia está en la palabra clave 'this' en el primer parámetro
    public static bool IsSimpleType(this Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        return underlyingType.IsPrimitive
            || underlyingType.IsEnum
            || underlyingType == typeof(string)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(DateTime)
            || underlyingType == typeof(Guid);
    }
}