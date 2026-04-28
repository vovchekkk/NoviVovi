using System.Text.Json.Serialization;
using NoviVovi.Api.Infrastructure;

namespace NoviVovi.Api.Transitions.Responses;

[JsonConverter(typeof(TransitionResponseConverter))]
public abstract record TransitionResponse
{
}