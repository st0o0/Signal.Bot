using System.Text.Json;
using System.Text.Json.Serialization;

namespace Signal.Bot.Serialization;

internal class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
            {
                var seconds = reader.GetInt32();
                return TimeSpan.FromSeconds(seconds);
            }
            case JsonTokenType.String:
            {
                var stringValue = reader.GetString();
                if (int.TryParse(stringValue, out var seconds))
                {
                    return TimeSpan.FromSeconds(seconds);
                }

                break;
            }
            case JsonTokenType.None:
            case JsonTokenType.StartObject:
            case JsonTokenType.EndObject:
            case JsonTokenType.StartArray:
            case JsonTokenType.EndArray:
            case JsonTokenType.PropertyName:
            case JsonTokenType.Comment:
            case JsonTokenType.True:
            case JsonTokenType.False:
            case JsonTokenType.Null:
            default:
                break;
        }

        throw new JsonException("Invalid expiresInSeconds format");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value.TotalSeconds);
    }
}