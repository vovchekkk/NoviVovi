using NoviVovi.Api.Steps.Requests;
using NoviVovi.Application.Steps.Features.Add;
using NoviVovi.Application.Steps.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.CommandMappers;

[Mapper]
public partial class StepCommandMapper
{
    public partial AddHideCharacterStepCommand ToCommand(AddHideCharacterStepRequest source, Guid novelId, Guid labelId);
    
    public partial AddJumpStepCommand ToCommand(AddJumpStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowBackgroundStepCommand ToCommand(AddShowBackgroundStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowCharacterStepCommand ToCommand(AddShowCharacterStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowMenuStepCommand ToCommand(AddShowMenuStepRequest source, Guid novelId, Guid labelId);
    
    public partial AddShowReplicaStepCommand ToCommand(AddShowReplicaStepRequest source, Guid novelId, Guid labelId);
    
    public partial PatchHideCharacterStepCommand ToCommand(PatchHideCharacterStepRequest source, Guid novelId, Guid labelId, Guid stepId);
    
    public partial PatchJumpStepCommand ToCommand(PatchJumpStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowBackgroundStepCommand ToCommand(PatchShowBackgroundStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowCharacterStepCommand ToCommand(PatchShowCharacterStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowMenuStepCommand ToCommand(PatchShowMenuStepRequest source, Guid novelId, Guid labelId, Guid stepId);
    
    public partial PatchShowReplicaStepCommand ToCommand(PatchShowReplicaStepRequest source, Guid novelId, Guid labelId, Guid stepId);
}