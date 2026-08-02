using GenericParserApi.Models;
using GenericParserApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenericParserApi.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private readonly IEnumerable<IParserStrategy> _parserStrategy;
        public ParserController(IEnumerable<IParserStrategy> parserStrategy)
        {
            _parserStrategy = parserStrategy;
        }
        [HttpPost("parse-content")]
        public IActionResult ParseContent([FromBody] ParseRequest request)
        {
            try
            {
                var base64 = Convert.FromBase64String(request.content);
                var decodedContent = System.Text.Encoding.UTF8.GetString(base64);

                var parser = _parserStrategy.FirstOrDefault(p => p.SupportedDataType == (SupportedDataTypes)request.Type);
                if (parser == null)
                {
                    return BadRequest($"No parser found for data type: {request.Type}");
                }

                var parsedData = parser.Parse(decodedContent).ToList();

                var response = new ParseResponse(
                    Status: "Successfully parsed",
                    TotalRecords: parsedData.Count,
                    Records: parsedData
                );

                return Ok(response);

            }
            catch (FormatException)
            {
                return BadRequest("Invalid base64 string.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while parsing the content: {ex.Message}");
            }
        }
    }
}
