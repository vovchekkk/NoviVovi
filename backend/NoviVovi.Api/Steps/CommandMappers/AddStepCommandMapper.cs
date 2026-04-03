using NoviVovi.Api.Steps.Requests;
using NoviVovi.Application.Steps.Features.Add;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.CommandMappers;

[Mapper]
public partial class AddStepCommandMapper
{
    public partial AddHideCharacterStepCommand ToCommand(AddHideCharacterStepRequest source, Guid novelId, Guid labelId);
    
    public partial AddJumpStepCommand ToCommand(AddJumpStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowBackgroundStepCommand ToCommand(AddShowBackgroundStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowCharacterStepCommand ToCommand(AddShowCharacterStepRequest source, Guid novelId, Guid labelId);

    public partial AddShowMenuStepCommand ToCommand(AddShowMenuStepRequest source, Guid novelId, Guid labelId);
    
    public partial AddShowReplicaStepCommand ToCommand(AddShowReplicaStepRequest source, Guid novelId, Guid labelId);
}