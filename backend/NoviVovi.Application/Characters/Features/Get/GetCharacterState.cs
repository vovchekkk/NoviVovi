using MediatR;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterStateQuery(
    Guid NovelId,
    Guid CharacterId,
    Guid StateId
) : IRequest<CharacterStateDto>;

public class GetCharacterStateHandler(
    INovelRepository novelRepository,
    ICharacterRepository characterRepository,
    CharacterStateDtoMapper mapper
) : IRequestHandler<GetCharacterStateQuery, CharacterStateDto>
{
    public async Task<CharacterStateDto> Handle(GetCharacterStateQuery request, CancellationToken ct)
    {
        var character = await characterRepository.GetByIdAsync(request.CharacterId, ct)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

        var state = character.CharacterStates.FirstOrDefault(c => c.Id == request.StateId)
                    ?? throw new NotFoundException($"Состояние персонажа '{request.StateId}' не найдено");

        return mapper.ToDto(state);
    }
}