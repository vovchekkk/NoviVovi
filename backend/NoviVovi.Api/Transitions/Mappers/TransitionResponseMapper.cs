using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class TransitionResponseMapper
{
    [MapDerivedType(typeof(ChoiceTransitionDto), typeof(ChoiceTransitionResponse))]
    [MapDerivedType(typeof(JumpTransitionDto), typeof(JumpTransitionResponse))]
    [MapDerivedType(typeof(NextStepTransitionDto), typeof(NextStepTransitionResponse))]
    public partial TransitionResponse ToResponse(TransitionDto source);
}