using System.Text.Json.Serialization;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ShowReplicaStepResponse), "show_replica")]
[JsonDerivedType(typeof(ShowCharacterStepResponse), "show_character")]
[JsonDerivedType(typeof(HideCharacterStepResponse), "hide_character")]
[JsonDerivedType(typeof(ShowBackgroundStepResponse), "show_background")]
[JsonDerivedType(typeof(ShowMenuStepResponse), "show_menu")]
[JsonDerivedType(typeof(JumpStepResponse), "jump")]
public abstract record StepResponse
{
    public required Guid Id { get; init; }
}

public abstract record StepResponse<TTransition> : StepResponse 
    where TTransition : TransitionResponse
{
    public required TTransition Transition { get; init; }
}