using System.Text.Json.Serialization;

namespace NoviVovi.Api.Transitions.Responses;

public record NextStepTransitionResponse : TransitionResponse
{
    // This property ensures the object is not empty and type discriminator is included
    [JsonPropertyName("$type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Type => "next_step";
}
