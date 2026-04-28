using System.Text.Json.Serialization;
using NoviVovi.Api.Infrastructure;

namespace NoviVovi.Api.Steps.Responses;

[JsonConverter(typeof(StepResponseConverter))]
public abstract record StepResponse
{
    public required Guid Id { get; init; }
}
