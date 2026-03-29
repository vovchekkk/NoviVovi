using NoviVovi.Application.Transitions.Contracts;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class NextStepTransitionSnapshotMapper
{
    public partial NextStepTransitionSnapshot ToSnapshot(NextStepTransition? subject);
    
    public partial IEnumerable<NextStepTransitionSnapshot> ToSnapshots(IEnumerable<NextStepTransition> subjects);
}