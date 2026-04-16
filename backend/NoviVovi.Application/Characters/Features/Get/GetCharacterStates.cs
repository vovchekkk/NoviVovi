using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterStatesQuery(
    Guid NovelId,
    Guid CharacterId
) : IRequest<IEnumerable<CharacterStateDto>>;

public class GetCharacterStatesHandler(
    INovelRepository novelRepository,
    CharacterStateDtoMapper mapper
) : IRequestHandler<GetCharacterStatesQuery, IEnumerable<CharacterStateDto>>
{
    public async Task<IEnumerable<CharacterStateDto>> Handle(GetCharacterStatesQuery request, CancellationToken ct)
    {
        var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);
        var character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

        var states = character.CharacterStates;

        return mapper.ToDtos(states);
    }
}