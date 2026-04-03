using NoviVovi.Api.Characters.Requests;
using NoviVovi.Application.Characters.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.CommandMappers;

[Mapper]
public partial class PatchCharacterCommandMapper
{
    public partial PatchCharacterCommand ToCommand(PatchCharacterRequest request, Guid novelId, Guid characterId);
}