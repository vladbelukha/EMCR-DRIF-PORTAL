using System.Text.Json;
using System.Text.Json.Serialization;

namespace EMCR.DRR.API.Utilities.Converters
{
    public class NullableDecimalConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                if (decimal.TryParse(value, out decimal result))
                {
                    return result;
                }
                throw new JsonException($"Invalid decimal value: {value}");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

    public class NullableIntConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                if (int.TryParse(value, out int result))
                {
                    return result;
                }
                throw new JsonException($"Invalid int value: {value}");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
