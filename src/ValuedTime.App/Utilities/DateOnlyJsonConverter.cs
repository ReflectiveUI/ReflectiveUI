using System.Text.Json;
using System.Text.Json.Serialization;
using NS = Newtonsoft.Json;

namespace ValuedTime.App.Utilities;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O");
        writer.WriteStringValue(isoDate);
    }
}

public class DateOnlyNewtonsoftJsonConverter : NS.JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        var match = objectType == typeof(DateOnly);
        return match;
    }

    public override object? ReadJson(NS.JsonReader reader, Type objectType, object? existingValue, NS.JsonSerializer serializer)
    {
        return DateOnly.Parse(reader.ReadAsString()!);
    }

    public override void WriteJson(NS.JsonWriter writer, object? value, NS.JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var isoDate = ((DateOnly)value).ToString("O");
        writer.WriteValue(isoDate);
    }
}