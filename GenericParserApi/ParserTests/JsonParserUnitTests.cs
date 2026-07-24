using GenericParserApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Xunit;

namespace ParserTests
{
    public class JsonParserUnitTests
    {
        [Fact]
        public void InternalJsonDataTypeShouldReturnCorrectlyParsedData()
        {
            // Arrange
            var strategy = new JsonParserStrategy();
            string rawJson = """[{"name": "John", "Age": "25"}, {"name": "Anna", "Age": "24"}]""";

            // Act
            var result = strategy.Parse(rawJson).ToList();

            // Assert
            Assert.Equal(2, result.Count);


            var firstItem = Assert.IsType<JsonElement>(result[0]);
            Assert.Equal("John", firstItem.GetProperty("name").GetString());
            Assert.Equal("25", firstItem.GetProperty("Age").GetString());

            var secondItem = Assert.IsType<JsonElement>(result[1]);
            Assert.Equal("Anna", secondItem.GetProperty("name").GetString());
            Assert.Equal("24", secondItem.GetProperty("Age").GetString());
        }

        [Fact]
        public void InternalJsonDataTypeShouldReturnSingleObjectForNonArrayJson()
        {
            // Arrange
            var strategy = new JsonParserStrategy();
            string rawJson = """{"name": "John", "Age": "25"}""";
            // Act
            var result = strategy.Parse(rawJson).ToList();
            // Assert
            Assert.Single(result);
            var item = Assert.IsType<JsonElement>(result[0]);
            Assert.Equal("John", item.GetProperty("name").GetString());
            Assert.Equal("25", item.GetProperty("Age").GetString());
        }

        [Fact]
        public void InternalJsonDataTypeShouldThrowExceptionForInvalidJson()
        {
            // Arrange
            var strategy = new JsonParserStrategy();
            string rawJson = """{"name": "John", "Age": "25"""; // Invalid JSON
            // Act & Assert
            Assert.Throws<JsonException>(() => strategy.Parse(rawJson).ToList());
        }
    }


}