using System.Text.Json.Serialization;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record HideCharacterStepResponse : StepResponse
{
    public required CharacterResponse Character { get; init; }
    
    [JsonConverter(typeof(TransitionResponseConverter))]
    public required TransitionResponse Transition { get; init; }
}
