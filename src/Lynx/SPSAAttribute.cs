using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lynx;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal class SPSAAttribute<T> : Attribute
    where T : INumberBase<T>, IMultiplyOperators<T, T, T>, IConvertible, IParsable<T>, ISpanParsable<T>
{
    private static readonly T _hundred;

    public T MinValue { get; }
    public T MaxValue { get; }
    public T Step { get; }

#pragma warning disable S3963 // "static" fields should be initialized inline
    static SPSAAttribute()
#pragma warning restore S3963 // "static" fields should be initialized inline
    {
        _hundred = T.Zero;
        for (int i = 0; i < 100; i++)
        {
            _hundred += T.One;
        }
    }

    public SPSAAttribute(T minValue, T maxValue, T step)
    {
        if (typeof(T) == typeof(double))
        {
            minValue *= _hundred;
            maxValue *= _hundred;
            step *= _hundred;
        }

        MinValue = minValue;
        MaxValue = maxValue;
        Step = step;
    }

    public string ToOBString(PropertyInfo property)
    {
        T val = GetPropertyValue(property);

        return $"{property.Name}, int, {val}, {MinValue}, {MaxValue}, {Step}, {Configuration.EngineSettings.SPSA_OB_R_end}";
    }

    private static T GetPropertyValue(PropertyInfo property)
    {
        T val = (T)property.GetValue(Configuration.EngineSettings)!;

        if (typeof(T) == typeof(double))
        {
            val *= _hundred;
        }

        return val;
    }

    public string ToOBPrettyString(PropertyInfo property)
    {
        T val = GetPropertyValue(property);

        return $"{property.Name,-35} {"int",-5} {val,-5} {MinValue,-5} {MaxValue,-5} {Step,-5} {Configuration.EngineSettings.SPSA_OB_R_end,-5}";
    }

    public KeyValuePair<string, JsonNode?> ToWeatherFactoryString(PropertyInfo property)
    {
        T val = GetPropertyValue(property);

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return KeyValuePair.Create(
            property.Name,
            JsonSerializer.SerializeToNode(new
            {
                value = val,
                min_value = MinValue,
                max_value = MaxValue,
                step = Step,
            }));
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }
}
