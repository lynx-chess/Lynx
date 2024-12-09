//HintName: GeneratedPackArrayAttribute.g.cs
#pragma warning disable IDE1006 // Naming Styles

using System;

namespace Lynx.Generator
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class GeneratedPackArrayAttribute : Attribute
    {
        public (short mg, short eg)[] Values { get; init; }

        public GeneratedPackArrayAttribute(params string[] values)
        {
            Values = values.Select(static s =>
            {
                var middle = s.IndexOf(',');

                return (short.Parse(s[..middle]), short.Parse(s[(middle + 1)..]));
            }).ToArray();
        }
    }
}

#pragma warning restore IDE1006 // Naming Styles