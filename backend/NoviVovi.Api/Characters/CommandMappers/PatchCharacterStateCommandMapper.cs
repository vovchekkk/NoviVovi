using NoviVovi.Api.Characters.Requests;
using NoviVovi.Application.Characters.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.CommandMappers;

[Mapper]
public partial class PatchCharacterStateCommandMapper
{
    public partial PatchCharacterStateCommand ToCommand(PatchCharacterStateRequest request, Guid novelId, Guid characterId, Guid stateId);
}