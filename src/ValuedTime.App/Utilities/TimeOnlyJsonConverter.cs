using System.Text.Json;
using System.Text.Json.Serialization;
using NS = Newtonsoft.Json;

namespace ValuedTime.App.Utilities;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeOnly.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        var isoTime = value.ToString("O");
        writer.WriteStringValue(isoTime);
    }
}

public class TimeOnlyNewtonsoftJsonConverter : NS.JsonConverter<TimeOnly>
{
    public override TimeOnly ReadJson(NS.JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, NS.JsonSerializer serializer)
    {
        return TimeOnly.Parse(reader.ReadAsString()!);
    }

    public override void WriteJson(NS.JsonWriter writer, TimeOnly value, NS.JsonSerializer serializer)
    {
        var isoDate = value.ToString("O");
        writer.WriteValue(isoDate);
    }
}