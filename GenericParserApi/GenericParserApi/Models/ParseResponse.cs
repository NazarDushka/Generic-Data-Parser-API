namespace GenericParserApi.Models
{
    public record ParseRespone (
    string Status,
    int TotalRecords,
    IEnumerable<object> Records
    );
}
