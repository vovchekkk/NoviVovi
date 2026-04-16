using System.Text.Json.Serialization;

namespace NoviVovi.Api.Images.Responses;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImageTypeResponse
{
    Background,
    Character,
    Default
}