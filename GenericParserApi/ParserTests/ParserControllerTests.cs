using GenericParserApi.Controllers;
using GenericParserApi.Models;
using GenericParserApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParserTests
{
    public class ParserControllerTests
    {
        [Fact]
        public void ParseController_ValidJSONTypeRequest_ShouldReturnOkWithCorrectlyParsedData()
        {
            // Arrange
            string jsonContent = """[{"name": "John", "Age": "25"}, {"name": "Anna", "Age": "24"}]""";
            string rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonContent));

            var request = new ParseRequest(
                Type: SupportedDataTypes.INTERNAL_JSON,
                content: rawData
            );

            var JsonStrategy = new JsonParserStrategy();
            var parserStrategies = new List<IParserStrategy> { JsonStrategy };
            var controller = new ParserController(parserStrategies);

            // Act 
            var result = controller.ParseContent(request);
            var parsedResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(parsedResult);
            Assert.Equal(200, parsedResult.StatusCode);

            var response = Assert.IsType<ParseResponse>(parsedResult.Value);
            Assert.Equal("Successfully parsed", response.Status);
            Assert.Equal(2, response.TotalRecords);

            var records = response.Records.ToList();

            var firstRecord = Assert.IsType<JsonElement>(records[0]);
            Assert.Equal("John", firstRecord.GetProperty("name").GetString());
            Assert.Equal("25", firstRecord.GetProperty("Age").GetString());

            var secondRecord = Assert.IsType<JsonElement>(records[1]);
            Assert.Equal("Anna", secondRecord.GetProperty("name").GetString());
            Assert.Equal("24", secondRecord.GetProperty("Age").GetString());
        }

        [Fact]
        public void ParseController_ValidCSVTypeRequest_ShouldReturnOkWithCorrectlyParsedData()
        { 
            string csvContent = "name,Age\nJohn,25\nAnna,24";

            string rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes(csvContent));

            var request = new ParseRequest(
                Type: SupportedDataTypes.CSV,
                content: rawData
            );

            var csvStrategy = new CSVParserStrategy();
            var parserStrategies = new List<IParserStrategy> { csvStrategy };
            var controller = new ParserController(parserStrategies);

            // Act 

            var result = controller.ParseContent(request);
            var parsedResult = result as OkObjectResult;

            // Assert

            Assert.NotNull(parsedResult);
            Assert.Equal(200, parsedResult.StatusCode);

            var response = Assert.IsType<ParseResponse>(parsedResult.Value);
            Assert.Equal("Successfully parsed", response.Status);
            Assert.Equal(2, response.TotalRecords);

            var records = response.Records.ToList();

            var firstRecord = Assert.IsType<Dictionary<string, string>>(records[0]);
            Assert.Equal("John", firstRecord["name"]);
            Assert.Equal("25", firstRecord["Age"]);

            var secondRecord = Assert.IsType<Dictionary<string, string>>(records[1]);
            Assert.Equal("Anna", secondRecord["name"]);
            Assert.Equal("24", secondRecord["Age"]);
        }

        [Fact]
        public void ParseController_InvalidBase64Request_ShouldReturnBadRequest()
        {
            // Arrange

            var request = new ParseRequest(
                Type: SupportedDataTypes.INTERNAL_JSON,
                content: "InvalidBase64String"
            );
            var JsonStrategy = new JsonParserStrategy();
            var parserStrategies = new List<IParserStrategy> { JsonStrategy };
            var controller = new ParserController(parserStrategies);

            // Act

            var result = controller.ParseContent(request);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert

            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Invalid base64 string.", badRequestResult.Value);
        }

        [Fact]
        public void ParseController_UnsupportedDataType_ShouldReturnBadRequest()
        {
            // Arrange

            string jsonContent = """[{"name": "John", "Age": "25"}, {"name": "Anna", "Age": "24"}]""";
            string rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonContent));

            var request = new ParseRequest(
                Type: SupportedDataTypes.CSV, 
                content: rawData
            );

            var JsonStrategy = new JsonParserStrategy();
            var parserStrategies = new List<IParserStrategy> { JsonStrategy };
            var controller = new ParserController(parserStrategies);

            // Act

            var result = controller.ParseContent(request);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert

            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal($"No parser found for data type: {request.Type}", badRequestResult.Value);
        }

        [Fact]
        public void ParseController_EmptyCSVContent_ShouldReturnBadRequest()
        {
            // Arrange

            string csvContent = "";
            string rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes(csvContent));

            var request = new ParseRequest(
                Type: SupportedDataTypes.CSV,
                content: rawData
            );

            var csvStrategy = new CSVParserStrategy();
            var parserStrategies = new List<IParserStrategy> { csvStrategy };
            var controller = new ParserController(parserStrategies);

            // Act

            var result = controller.ParseContent(request);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert

            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("CSV data must contain at least a header and one record.", badRequestResult.Value);
        }

        [Fact]
        public void ParseController_CSVWithMismatchedRecord_ShouldReturnBadRequest()
        {
            // Arrange

            string csvContent = "name,Age\nJohn,25\nAnna"; // Missing Age for Anna
            string rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes(csvContent));
            var request = new ParseRequest(
                Type: SupportedDataTypes.CSV,
                content: rawData
            );

            var csvStrategy = new CSVParserStrategy();
            var parserStrategies = new List<IParserStrategy> { csvStrategy };
            var controller = new ParserController(parserStrategies);

            // Act

            var result = controller.ParseContent(request);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert

            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Record 2 does not match header length.", badRequestResult.Value);
        }

        [Fact]
        public void ParseController_InvalidJSONContent_ShouldReturnBadRequest()
        {
            // Arrange
            string jsonContent = """{"name": "John", "Age": "25"""; // Invalid JSON
            string rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonContent));
            var request = new ParseRequest(
                Type: SupportedDataTypes.INTERNAL_JSON,
                content: rawData
            );
            var JsonStrategy = new JsonParserStrategy();
            var parserStrategies = new List<IParserStrategy> { JsonStrategy };
            var controller = new ParserController(parserStrategies);
            // Act
            var result = controller.ParseContent(request);
            var badRequestResult = result as BadRequestObjectResult;
            // Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Invalid JSON data.", badRequestResult.Value.ToString());
        }

        [Fact]
        public void ParseController_EmptyJSONContent_ShouldReturnBadRequest()
        {
            // Arrange

            string jsonContent = "";
            string rawData = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonContent));
            var request = new ParseRequest(
                Type: SupportedDataTypes.INTERNAL_JSON,
                content: rawData
            );

            var JsonStrategy = new JsonParserStrategy();
            var parserStrategies = new List<IParserStrategy> { JsonStrategy };
            var controller = new ParserController(parserStrategies);

            // Act

            var result = controller.ParseContent(request);
            var badRequestResult = result as BadRequestObjectResult;

            // Assert

            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Invalid JSON data.", badRequestResult.Value.ToString());
        }

    }

   
}

