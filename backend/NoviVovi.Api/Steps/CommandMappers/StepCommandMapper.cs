using NoviVovi.Api.Steps.Requests;
using NoviVovi.Application.Steps.Features.Add;
using NoviVovi.Application.Steps.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.CommandMappers;

[Mapper]
public partial class StepCommandMapper
{
    /// <summary>
    /// Маппит AddStepRequest в соответствующую команду без использования dynamic.
    /// </summary>
    public AddStepCommand ToCommand(AddStepRequest request, Guid novelId, Guid labelId)
    {
        return request switch
        {
            AddHideCharacterStepRequest r => ToCommand(r, novelId, labelId),
            AddJumpStepRequest r => ToCommand(r, novelId, labelId),
            AddShowBackgroundStepRequest r => ToCommand(r, novelId, labelId),
            AddShowCharacterStepRequest r => ToCommand(r, novelId, labelId),
            AddShowMenuStepRequest r => ToCommand(r, novelId, labelId),
            AddShowReplicaStepRequest r => ToCommand(r, novelId, labelId),
            _ => throw new ArgumentException($"Unknown request type: {request.GetType().Name}")
        };
    }
    
    public partial AddHideCharacterStepCommand ToCommand(AddHideCharacterStepRequest source, Guid novelId, Guid labelId);
    
    public partial AddJumpStepCommand ToCommand(AddJumpStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowBackgroundStepCommand ToCommand(AddShowBackgroundStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowCharacterStepCommand ToCommand(AddShowCharacterStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowMenuStepCommand ToCommand(AddShowMenuStepRequest source, Guid novelId, Guid labelId);
    
    public partial AddShowReplicaStepCommand ToCommand(AddShowReplicaStepRequest source, Guid novelId, Guid labelId);
    
    public PatchStepCommand ToCommand(PatchStepRequest request, Guid novelId, Guid labelId, Guid stepId)
    {
        return request switch
        {
            PatchHideCharacterStepRequest r => ToCommand(r, novelId, labelId, stepId),
            PatchJumpStepRequest r => ToCommand(r, novelId, labelId, stepId),
            PatchShowBackgroundStepRequest r => ToCommand(r, novelId, labelId, stepId),
            PatchShowCharacterStepRequest r => ToCommand(r, novelId, labelId, stepId),
            PatchShowMenuStepRequest r => ToCommand(r, novelId, labelId, stepId),
            PatchShowReplicaStepRequest r => ToCommand(r, novelId, labelId, stepId),
            _ => throw new ArgumentException($"Unknown request type: {request.GetType().Name}")
        };
    }
    
    public partial PatchHideCharacterStepCommand ToCommand(PatchHideCharacterStepRequest source, Guid novelId, Guid labelId, Guid stepId);
    
    public partial PatchJumpStepCommand ToCommand(PatchJumpStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowBackgroundStepCommand ToCommand(PatchShowBackgroundStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowCharacterStepCommand ToCommand(PatchShowCharacterStepRequest source, Guid novelId, Guid labelId, Guid stepId);

    public partial PatchShowMenuStepCommand ToCommand(PatchShowMenuStepRequest source, Guid novelId, Guid labelId, Guid stepId);
    
    public partial PatchShowReplicaStepCommand ToCommand(PatchShowReplicaStepRequest source, Guid novelId, Guid labelId, Guid stepId);
}