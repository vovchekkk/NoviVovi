using NoviVovi.Api.Characters.Responses;
using NoviVovi.Application.Characters.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.Mappers;

[Mapper]
public partial class CharacterResponseMapper
{
    public partial CharacterResponse ToResponse(CharacterDto subject);
    
    public partial IEnumerable<CharacterResponse> ToResponses(IEnumerable<CharacterDto> subjects);
}