using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class TransitionResponseMapper
{
    public partial NextStepTransitionResponse ToSnapshot(NextStepTransitionSnapshot novel);
    
    public partial JumpTransitionResponse ToSnapshot(JumpTransitionSnapshot novel);
    
    public partial ChoiceTransitionResponse ToSnapshot(ChoiceTransitionSnapshot novel);
}