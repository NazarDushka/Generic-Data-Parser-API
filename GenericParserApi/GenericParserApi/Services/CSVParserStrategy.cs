using GenericParserApi.Models;

namespace GenericParserApi.Services

{
    public class CSVParserStrategy: IParserStrategy
    {
        public SupportedDataTypes SupportedDataType => SupportedDataTypes.CSV;
        public IEnumerable<object> Parse(string rawData)
        {
            var records = new List<Dictionary<string, string>>();
            var lines = rawData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
            {
                throw new ArgumentException("CSV data must contain at least a header and one record.");
            }
            var headers = lines[0].Split(',');
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                if (values.Length != headers.Length)
                {
                    throw new ArgumentException($"Record {i} does not match header length.");
                }
                var record = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length; j++)
                {
                    record[headers[j]] = values[j];
                }
                records.Add(record);
            }
            return records;
        }
    }
}
