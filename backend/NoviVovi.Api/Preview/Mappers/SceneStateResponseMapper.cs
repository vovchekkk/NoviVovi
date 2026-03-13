using NoviVovi.Api.Preview.Responses;
using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Transitions.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Preview.Mappers;

[Mapper]
public partial class SceneStateResponseMapper
{
    public partial SceneStateResponse ToSnapshot(SceneStateSnapshot novel);

    public partial IEnumerable<SceneStateResponse> ToSnapshots(IEnumerable<SceneStateSnapshot> novels);

    public partial NextStepTransitionResponse ToSnapshot(NextStepTransitionSnapshot novel);
    
    public partial JumpTransitionResponse ToSnapshot(JumpTransitionSnapshot novel);
    
    public partial ChoiceTransitionResponse ToSnapshot(ChoiceTransitionSnapshot novel);
}