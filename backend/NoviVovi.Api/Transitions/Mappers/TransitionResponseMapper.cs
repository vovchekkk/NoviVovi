using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class TransitionResponseMapper
{
    [MapDerivedType(typeof(NextStepTransitionSnapshot), typeof(NextStepTransitionResponse))]
    [MapDerivedType(typeof(JumpTransitionSnapshot), typeof(JumpTransitionResponse))]
    [MapDerivedType(typeof(ChoiceTransitionSnapshot), typeof(ChoiceTransitionResponse))]
    public partial TransitionResponse ToSnapshot(TransitionSnapshot novel);

    public partial NextStepTransitionResponse ToSnapshot(NextStepTransitionSnapshot novel);
    
    public partial JumpTransitionResponse ToSnapshot(JumpTransitionSnapshot novel);
    
    public partial ChoiceTransitionResponse ToSnapshot(ChoiceTransitionSnapshot novel);
}