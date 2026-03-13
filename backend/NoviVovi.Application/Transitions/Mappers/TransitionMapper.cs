using NoviVovi.Application.Transitions.Contracts;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class TransitionMapper
{
    public partial NextStepTransitionSnapshot ToSnapshot(NextStepTransition? novel);
    
    public partial JumpTransitionSnapshot ToSnapshot(JumpTransition? novel);
    
    public partial ChoiceTransitionSnapshot ToSnapshot(ChoiceTransition? novel);
}