using System.Text.Json.Serialization;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ShowReplicaStepDto), "show_replica")]
[JsonDerivedType(typeof(ShowCharacterStepDto), "show_character")]
[JsonDerivedType(typeof(HideCharacterStepDto), "hide_character")]
[JsonDerivedType(typeof(ShowBackgroundStepDto), "show_background")]
[JsonDerivedType(typeof(ShowMenuStepDto), "show_menu")]
[JsonDerivedType(typeof(JumpStepDto), "jump")]
public abstract record StepDto
{
    public required Guid Id { get; init; }
}

public abstract record StepDto<TTransition> : StepDto 
    where TTransition : TransitionDto
{
    public required TTransition Transition { get; init; }
}