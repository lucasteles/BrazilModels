using System.Buffers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrazilModels.Json;

sealed class StringSystemTextJsonConverter<T> : JsonConverter<T>
    where T : struct, IFormattable, IStringValue
#if NET8_0_OR_GREATER
    , IUtf8SpanFormattable, IUtf8SpanParsable<T>
#endif
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException(
                $"The JSON value could not be converted to {typeof(T)}");

#if NET8_0_OR_GREATER
        var utf8Bytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
        return T.Parse(utf8Bytes, CultureInfo.InvariantCulture);
#else
        return (T)Activator.CreateInstance(typeof(T), reader.GetString())!;
#endif
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value.IsEmpty)
        {
            writer.WriteNullValue();
            return;
        }

#if NET8_0_OR_GREATER
        Span<byte> buffer = stackalloc byte[T.ValueSize];
        if (!value.TryFormat(buffer, out var size, default, default))
            throw new JsonException(
                $"The JSON value {value} could not be written for {typeof(T)}");

        writer.WriteStringValue(buffer[..size]);
#else
        writer.WriteStringValue(value.ToString(null, null));
#endif
    }
}
