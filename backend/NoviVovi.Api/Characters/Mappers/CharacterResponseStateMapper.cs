using NoviVovi.Api.Characters.Responses;
using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Domain.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Characters.Mappers;

[Mapper]
public partial class CharacterStateResponseMapper
{
    public partial CharacterStateResponse ToSnapshot(CharacterStateSnapshot novel);

    public partial IEnumerable<CharacterStateResponse> ToSnapshots(IEnumerable<CharacterStateSnapshot> novels);
}