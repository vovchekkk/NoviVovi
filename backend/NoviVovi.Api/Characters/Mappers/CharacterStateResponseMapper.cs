using NoviVovi.Api.Characters.Responses;
using NoviVovi.Application.Characters.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.Mappers;

[Mapper]
public partial class CharacterStateResponseMapper
{
    public partial CharacterStateResponse ToResponse(CharacterStateDto source);

    public partial IEnumerable<CharacterStateResponse> ToResponses(IEnumerable<CharacterStateDto> sources);
}