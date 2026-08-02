using GenericParserApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GenericParserApi.Services
{
    public class JsonParserStrategy : IParserStrategy
    {
        public SupportedDataTypes SupportedDataType => SupportedDataTypes.INTERNAL_JSON;
        public IEnumerable<object> Parse(string rawData)
        {
            try
            {
                var records = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(rawData);
                if (records.ValueKind == JsonValueKind.Array)
                {
                    return records.EnumerateArray().Cast<object>().ToList();
                }

                return new List<object>() { records };
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Invalid JSON data.");
            }
        }
    }
    
}
