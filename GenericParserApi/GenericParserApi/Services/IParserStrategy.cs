using GenericParserApi.Models;
using System.ComponentModel.DataAnnotations;

namespace GenericParserApi.Services
{
    public interface IParserStrategy
    {
        SupportedDataTypes SupportedDataType { get; }
        IEnumerable<object> Parse(string rawData);
    }
}
