using MediatR;
using NoviVovi.Application.Characters.Abstactions;
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
    ICharacterRepository characterRepository,
    CharacterStateDtoMapper mapper
) : IRequestHandler<GetCharacterStatesQuery, IEnumerable<CharacterStateDto>>
{
    public async Task<IEnumerable<CharacterStateDto>> Handle(GetCharacterStatesQuery request, CancellationToken ct)
    {
        var character = await characterRepository.GetByIdAsync(request.CharacterId, ct)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

        var states = character.CharacterStates;

        return mapper.ToDtos(states);
    }
}