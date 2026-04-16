using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Characters.Features.Get;

public record GetCharacterQuery(
    Guid NovelId,
    Guid CharacterId
) : IRequest<CharacterDto>;

public class GetCharacterHandler(
    INovelRepository novelRepository,
    CharacterDtoMapper mapper
) : IRequestHandler<GetCharacterQuery, CharacterDto>
{
    public async Task<CharacterDto> Handle(GetCharacterQuery request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var character = novel.Characters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        
        return mapper.ToDto(character);
    }
}