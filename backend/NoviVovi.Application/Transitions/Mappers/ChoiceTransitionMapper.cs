using NoviVovi.Application.Transitions.Contracts;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class ChoiceTransitionMapper
{
    public partial ChoiceTransitionSnapshot ToSnapshot(ChoiceTransition? novel);
    
    public partial IEnumerable<ChoiceTransitionSnapshot> ToSnapshots(IEnumerable<ChoiceTransition> novels);
}