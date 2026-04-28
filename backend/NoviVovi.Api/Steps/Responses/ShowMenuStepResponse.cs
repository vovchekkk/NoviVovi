using System.Text.Json.Serialization;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Api.Menu.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowMenuStepResponse : StepResponse
{
    public required MenuResponse Menu { get; init; }
    
    [JsonConverter(typeof(TransitionResponseConverter))]
    public required TransitionResponse Transition { get; init; }
}
