using System.Text.Json.Serialization;

namespace NoviVovi.Api.Images.Requests;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ImageTypeRequest
{
    Background,
    Character,
    Default
}