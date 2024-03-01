using System.Dynamic;

namespace Razmx.Core;

public static class ObjectExtensions
{
    public static IDictionary<string, object?> ToExpando<T>(this T? obj)
    {
        if (obj is null)
            return new ExpandoObject();

        var expando = new ExpandoObject();

        var type = obj.GetType();

        foreach (var propertyInfo in type.GetProperties())
        {
            var currentValue = propertyInfo.GetValue(obj);
            expando.TryAdd(propertyInfo.Name, currentValue);
        }

        return expando;
    }
}