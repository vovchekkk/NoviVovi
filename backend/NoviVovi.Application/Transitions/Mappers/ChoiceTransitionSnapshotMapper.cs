using NoviVovi.Application.Transitions.Contracts;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class ChoiceTransitionSnapshotMapper
{
    public partial ChoiceTransitionSnapshot ToSnapshot(ChoiceTransition? subject);
    
    public partial IEnumerable<ChoiceTransitionSnapshot> ToSnapshots(IEnumerable<ChoiceTransition> subjects);
}