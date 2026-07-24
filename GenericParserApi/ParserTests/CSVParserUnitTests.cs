using System;
using GenericParserApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTests
{
    public class CSVParserUnitTests
    {
        [Fact]
        public void CSVDataTypeShouldReturnCorrectlyParsedData()
        {
            // Arrange
            var strategy = new CSVParserStrategy();
            string rawCsv = "name,Age\nJohn,25\nAnna,24";
            // Act
            var result = strategy.Parse(rawCsv).ToList();
            // Assert
            Assert.Equal(2, result.Count);
            var firstItem = Assert.IsType<Dictionary<string, string>>(result[0]);
            Assert.Equal("John", firstItem["name"]);
            Assert.Equal("25", firstItem["Age"]);
            var secondItem = Assert.IsType<Dictionary<string, string>>(result[1]);
            Assert.Equal("Anna", secondItem["name"]);
            Assert.Equal("24", secondItem["Age"]);
        }

        [Fact]
        public void CSVDataTypeShouldReturnExceptionForEmptyInput()
        {
            // Arrange
            var strategy = new CSVParserStrategy();
            string rawCsv = "";
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => strategy.Parse(rawCsv).ToList());
            Assert.Equal("CSV data must contain at least a header and one record.", exception.Message);
        }

        [Fact]
        public void CSVDataTypeShouldHandleMissingValues()
        {
            // Arrange
            var strategy = new CSVParserStrategy();
            string rawCsv = "name,Age\nJohn,25\nAnna,";
            // Act
            var result = strategy.Parse(rawCsv).ToList();
            // Assert
            Assert.Equal(2, result.Count);
            var secondItem = Assert.IsType<Dictionary<string, string>>(result[1]);
            Assert.Equal("Anna", secondItem["name"]);
            Assert.Equal(string.Empty, secondItem["Age"]);
        }


    }
}