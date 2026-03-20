using NoviVovi.Application.Transitions.Contracts;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class NextStepTransitionMapper
{
    public partial NextStepTransitionSnapshot ToSnapshot(NextStepTransition? novel);
    
    public partial IEnumerable<NextStepTransitionSnapshot> ToSnapshots(IEnumerable<NextStepTransition> novels);
}