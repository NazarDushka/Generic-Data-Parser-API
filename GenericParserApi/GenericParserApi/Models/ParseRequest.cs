using System.ComponentModel.DataAnnotations;

namespace GenericParserApi.Models;

    public record ParseRequest(DataType Type, string content);
    

