using NoviVovi.Api.Characters.Requests;
using NoviVovi.Application.Characters.Features.Add;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.CommandMappers;

[Mapper]
public partial class AddCharacterCommandMapper
{
    public partial AddCharacterCommand ToCommand(AddCharacterRequest request, Guid novelId);
}