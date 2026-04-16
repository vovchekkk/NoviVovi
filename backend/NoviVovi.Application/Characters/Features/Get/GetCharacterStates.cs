using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;

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
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var character = novel.Characters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

        var states = character.CharacterStates;

        return mapper.ToDtos(states);
    }
}