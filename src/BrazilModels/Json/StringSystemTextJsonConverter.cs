using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrazilModels.Json;

sealed class StringSystemTextJsonConverter<T> : JsonConverter<T> where T : struct, IFormattable
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options
    ) =>
        (T)Activator.CreateInstance(typeof(T), reader.GetString())!;

    public override void Write(
        Utf8JsonWriter writer, T value,
        JsonSerializerOptions options
    ) =>
        writer.WriteStringValue(value.ToString(null, null));
}
