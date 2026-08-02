namespace GenericParserApi.Models
{
    public record ParseResponse (
    string Status,
    int TotalRecords,
    IEnumerable<object> Records
    );
}
