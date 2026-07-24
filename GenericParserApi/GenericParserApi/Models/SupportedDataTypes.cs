using System.Text.Json.Serialization;

namespace GenericParserApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SupportedDataTypes
    {
        CSV,
        INTERNAL_JSON
    }
}
