using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AutoSplitDebugger;

internal class StringEnumDescriptionConverter : StringEnumConverter
{
    private static readonly Dictionary<Type, Dictionary<string, Enum>> _enumCache = new ();

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var isNullable = objectType == typeof(Nullable<>);
        if (reader.TokenType == JsonToken.Null)
        {
            if (isNullable)
            {
                return null;
            }

            throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
        }

        var t = isNullable ? Nullable.GetUnderlyingType(objectType)! : objectType;

        try
        {
            // only override the handling of the string values since
            // we just want to check the 
            if (reader.TokenType == JsonToken.String)
            {
                var enumText = reader.Value?.ToString();

                if (enumText == null)
                {
                    if (isNullable) return null;

                    throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
                }
                
                // cache the type's string map on the first use
                if (!_enumCache.ContainsKey(t))
                {
                    var names = new Dictionary<string, Enum>();
                    var values = Enum.GetValues(t);

                    foreach (var value in values)
                    {
                        var enumVal = (Enum) value;
                        var name = enumVal.GetDescription();
                        names[name] = enumVal;
                    }

                    _enumCache[t] = names;
                }

                var map = _enumCache[t];

                if (map.ContainsKey(enumText!))
                {
                    return map[enumText];
                }

                return Enum.Parse(t, enumText, true);
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
        catch (Exception ex)
        {
            throw new JsonSerializationException($"Error converting value {reader.Value} to type '{objectType}'.", ex);
        }
    }

}
