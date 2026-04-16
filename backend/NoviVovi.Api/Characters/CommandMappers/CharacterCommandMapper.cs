using NoviVovi.Api.Characters.Requests;
using NoviVovi.Application.Characters.Features.Add;
using NoviVovi.Application.Characters.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.CommandMappers;

[Mapper]
public partial class CharacterCommandMapper
{
    public partial AddCharacterCommand ToCommand(AddCharacterRequest request, Guid novelId);
    
    public partial PatchCharacterCommand ToCommand(PatchCharacterRequest request, Guid novelId, Guid characterId);
}