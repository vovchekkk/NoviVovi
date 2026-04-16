using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterStateQuery(
    Guid NovelId,
    Guid CharacterId,
    Guid StateId
) : IRequest<CharacterStateDto>;

public class GetCharacterStateHandler(
    INovelRepository novelRepository,
    CharacterStateDtoMapper mapper
) : IRequestHandler<GetCharacterStateQuery, CharacterStateDto>
{
    public async Task<CharacterStateDto> Handle(GetCharacterStateQuery request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var character = novel.Characters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

        var state = character.CharacterStates.FirstOrDefault(c => c.Id == request.StateId)
                    ?? throw new NotFoundException($"Состояние персонажа '{request.StateId}' не найдено");

        return mapper.ToDto(state);
    }
}