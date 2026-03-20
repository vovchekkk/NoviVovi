using NoviVovi.Application.Transitions.Contracts;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class JumpTransitionMapper
{
    public partial JumpTransitionSnapshot ToSnapshot(JumpTransition? subject);
    
    public partial IEnumerable<JumpTransitionSnapshot> ToSnapshots(IEnumerable<JumpTransition> subjects);
}