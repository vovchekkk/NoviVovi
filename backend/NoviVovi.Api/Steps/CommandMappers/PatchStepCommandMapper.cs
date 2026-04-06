using NoviVovi.Api.Steps.Requests;
using NoviVovi.Application.Steps.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.CommandMappers;

[Mapper]
public partial class PatchStepCommandMapper
{
    public partial PatchHideCharacterStepCommand ToCommand(PatchHideCharacterStepRequest source, Guid novelId, Guid labelId, Guid stepId);
    
    public partial PatchJumpStepCommand ToCommand(PatchJumpStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowBackgroundStepCommand ToCommand(PatchShowBackgroundStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowCharacterStepCommand ToCommand(PatchShowCharacterStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowMenuStepCommand ToCommand(PatchShowMenuStepRequest source, Guid novelId, Guid labelId, Guid stepId);
    
    public partial PatchShowReplicaStepCommand ToCommand(PatchShowReplicaStepRequest source, Guid novelId, Guid labelId, Guid stepId);
}