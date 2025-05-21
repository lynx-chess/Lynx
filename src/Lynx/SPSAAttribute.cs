using NLog;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lynx;

#pragma warning disable IDE1006 // Naming Styles
internal sealed record WeatherFactoryOutput<T>(T value, T min_value, T max_value, double step);
#pragma warning restore IDE1006 // Naming Styles

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal sealed class SPSAAttribute<T> : Attribute
    where T : INumberBase<T>, IMultiplyOperators<T, T, T>, IConvertible, IParsable<T>, ISpanParsable<T>, IDivisionOperators<T, T, T>, IMinMaxValue<T>
{
#pragma warning disable S2743 // Static fields should not be used in generic types
#pragma warning disable RCS1158 // Static member in generic type should use a type parameter
    internal static readonly JsonSerializerOptions _jsonSerializerOptions = new() { TypeInfoResolver = EngineSettingsJsonSerializerContext.Default };
#pragma warning restore RCS1158 // Static member in generic type should use a type parameter
#pragma warning restore S2743 // Static fields should not be used in generic types

    private static readonly T _hundred;

    public T MinValue { get; }
    public T MaxValue { get; }
    public double Step { get; }
    public bool Enabled { get; }

#pragma warning disable S3963, CA1810 // "static" fields should be initialized inline
    static SPSAAttribute()
#pragma warning restore S3963, CA1810 // "static" fields should be initialized inline
    {
        _hundred = T.Zero;
        for (int i = 0; i < 100; i++)
        {
            _hundred += T.One;
        }
    }

    public SPSAAttribute(bool enabled)
        : this(T.MinValue, T.MaxValue, default, enabled)
    {
    }

    public SPSAAttribute(T minValue, T maxValue, double step, bool enabled = true)
    {
        if (typeof(T) == typeof(double))
        {
            minValue *= _hundred;
            maxValue *= _hundred;
            step *= 100;
        }

        MinValue = minValue;
        MaxValue = maxValue;
        Step = step;
        Enabled = enabled;
    }

    public string ToOBString(PropertyInfo property)
    {
        T val = GetPropertyValue(property);

        return $"{property.Name}, int, {val}, {MinValue}, {MaxValue}, {Step}, {Configuration.EngineSettings.SPSA_OB_R_end}";
    }

    internal static T GetPropertyValue(PropertyInfo property)
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
        var percentage = 100 * (Step / double.Parse((MaxValue - MinValue).ToString()!));

        return $"{property.Name,-35} {"int",-5} {val,-5} {MinValue,-5} {MaxValue,-5} {Step,-5} {$"{percentage:F2}%",-8}{Configuration.EngineSettings.SPSA_OB_R_end,-5}";
    }

    public KeyValuePair<string, JsonNode?> ToWeatherFactoryString(PropertyInfo property)
    {
        T val = GetPropertyValue(property);

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return KeyValuePair.Create(
            property.Name,
            JsonSerializer.SerializeToNode(
                new WeatherFactoryOutput<T>(val, MinValue, MaxValue, Step),
                _jsonSerializerOptions));
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }
}

public static class SPSAAttributeHelpers
{
    private const string PropertyHasMoreThanOne = "Property {0} has more than one [{1}]";
    private const string PropertyHasAnUnsupportedType = "Property {0} has a [{1}] defined with unsupported type <{2}>";
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static IEnumerable<string> GenerateOpenBenchStrings()
    {
        foreach (var property in typeof(EngineSettings).GetProperties())
        {
            var genericType = typeof(SPSAAttribute<>);
            var spsaArray = property.GetCustomAttributes(genericType).ToArray();
            var count = spsaArray.Length;

            if (count > 1)
            {
                _logger.Warn(PropertyHasMoreThanOne, property.Name, genericType.Name);
            }

            if (count == 0)
            {
                continue;
            }

            var genericSpsa = spsaArray[0];
            if (genericSpsa is SPSAAttribute<int> intSpsa)
            {
                yield return intSpsa.ToOBString(property);
            }
            else if (genericSpsa is SPSAAttribute<double> doubleSpsa)
            {
                yield return doubleSpsa.ToOBString(property);
            }
            else
            {
                _logger.Error(PropertyHasAnUnsupportedType, property.Name, genericSpsa);
            }
        }
    }

    public static IEnumerable<string> GenerateOpenBenchPrettyStrings()
    {
        foreach (var property in typeof(EngineSettings).GetProperties())
        {
            var genericType = typeof(SPSAAttribute<>);
            var spsaArray = property.GetCustomAttributes(genericType).ToArray();
            var count = spsaArray.Length;

            if (count > 1)
            {
                _logger.Warn(PropertyHasMoreThanOne, property.Name, genericType.Name);
            }

            if (count == 0)
            {
                continue;
            }

            var genericSpsa = spsaArray[0];
            if (genericSpsa is SPSAAttribute<int> intSpsa)
            {
                yield return intSpsa.ToOBPrettyString(property);
            }
            else if (genericSpsa is SPSAAttribute<double> doubleSpsa)
            {
                yield return doubleSpsa.ToOBPrettyString(property);
            }
            else
            {
                _logger.Error(PropertyHasAnUnsupportedType, property.Name, genericSpsa);
            }
        }
    }

    public static IEnumerable<KeyValuePair<string, JsonNode?>> GenerateWeatherFactoryStrings()
    {
        foreach (var property in typeof(EngineSettings).GetProperties())
        {
            var genericType = typeof(SPSAAttribute<>);
            var spsaArray = property.GetCustomAttributes(genericType).ToArray();
            var count = spsaArray.Length;

            if (count > 1)
            {
                _logger.Warn(PropertyHasMoreThanOne, property.Name, genericType.Name);
            }

            if (count == 0)
            {
                continue;
            }

            var genericSpsa = spsaArray[0];
            if (genericSpsa is SPSAAttribute<int> intSpsa)
            {
                yield return intSpsa.ToWeatherFactoryString(property);
            }
            else if (genericSpsa is SPSAAttribute<double> doubleSpsa)
            {
                yield return doubleSpsa.ToWeatherFactoryString(property);
            }
            else
            {
                _logger.Error(PropertyHasAnUnsupportedType, property.Name, genericSpsa);
            }
        }
    }

    public static IEnumerable<string> GenerateOptionStrings()
    {
        foreach (var property in typeof(EngineSettings).GetProperties())
        {
            var genericType = typeof(SPSAAttribute<>);
            var spsaArray = property.GetCustomAttributes(genericType).ToArray();
            var count = spsaArray.Length;

            if (count > 1)
            {
                _logger.Warn(PropertyHasMoreThanOne, property.Name, genericType.Name);
            }

            if (count == 0)
            {
                continue;
            }

            var genericSpsa = spsaArray[0];
            if (genericSpsa is SPSAAttribute<int> intSpsa)
            {
                int val = SPSAAttribute<int>.GetPropertyValue(property);

                yield return $"option name {property.Name} type spin default {val} min {intSpsa.MinValue} max {intSpsa.MaxValue}";
            }
            else if (genericSpsa is SPSAAttribute<double> doubleSpsa)
            {
                int val = (int)SPSAAttribute<double>.GetPropertyValue(property);

                yield return $"option name {property.Name} type spin default {val} min {doubleSpsa.MinValue} max {doubleSpsa.MaxValue}";
            }
            else
            {
                _logger.Error(PropertyHasAnUnsupportedType, property.Name, genericSpsa);
            }
        }
    }

    internal static bool ParseUCIOption(ReadOnlySpan<char> command, Span<Range> commandItems, ReadOnlySpan<char> firstWord, int length)
    {
        foreach (var property in typeof(EngineSettings).GetProperties())
        {
            if (!property.Name.Equals(firstWord, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var genericType = typeof(SPSAAttribute<>);
            var spsaArray = property.GetCustomAttributes(genericType).ToArray();
            var count = spsaArray.Length;

            if (count > 1)
            {
                _logger.Warn(PropertyHasMoreThanOne, property.Name, genericType.Name);
            }

            if (count == 0)
            {
                continue;
            }

            var genericSpsa = spsaArray[0];
            if (genericSpsa is SPSAAttribute<int> intSpsa)
            {
                if (length > 4 && int.TryParse(command[commandItems[4]], out var value))
                {
                    property.SetValue(Configuration.EngineSettings, value);

                    return true;
                }
            }
            else if (genericSpsa is SPSAAttribute<double> doubleSpsa)
            {
                if (length > 4 && double.TryParse(command[commandItems[4]], out var value))
                {
                    property.SetValue(Configuration.EngineSettings, 0.01 * value);

                    return true;
                }
            }
            else
            {
                _logger.Error(PropertyHasAnUnsupportedType, property.Name, genericSpsa);
            }
        }

        return false;
    }
}
