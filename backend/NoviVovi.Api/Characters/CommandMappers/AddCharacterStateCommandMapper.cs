using NoviVovi.Api.Characters.Requests;
using NoviVovi.Application.Characters.Features.Add;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.CommandMappers;

[Mapper]
public partial class AddCharacterStateCommandMapper
{
    public partial AddCharacterStateCommand ToCommand(AddCharacterStateRequest request, Guid novelId, Guid characterId);
}