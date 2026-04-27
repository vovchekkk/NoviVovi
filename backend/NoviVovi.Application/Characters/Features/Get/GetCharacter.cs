using MediatR;
using NoviVovi.Application.Characters.Abstactions;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterQuery(
    Guid NovelId,
    Guid CharacterId
) : IRequest<CharacterDto>;

public class GetCharacterHandler(
    INovelRepository novelRepository,
    ICharacterRepository characterRepository,
    CharacterDtoMapper mapper
) : IRequestHandler<GetCharacterQuery, CharacterDto>
{
    public async Task<CharacterDto> Handle(GetCharacterQuery request, CancellationToken ct)
    {
        var character = await characterRepository.GetByIdAsync(request.CharacterId, ct)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        
        return mapper.ToDto(character);
    }
}