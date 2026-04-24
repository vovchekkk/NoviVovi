using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Common;

namespace NoviVovi.Application.Characters.Features.Patch;

public record PatchCharacterCommand : IRequest<CharacterDto>
{
    public required Guid NovelId { get; init; }
    public required Guid CharacterId { get; init; }
    public string? Name { get; init; }
    public string? NameColor { get; init; }
    public string? Description { get; init; }
}

public class PatchCharacterHandler(
    INovelRepository novelRepository,
    IUnitOfWork unitOfWork,
    CharacterDtoMapper mapper
) : IRequestHandler<PatchCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(PatchCharacterCommand request, CancellationToken ct)
    {
        var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);
        var character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        
        if (request.Name is not null)
            character.UpdateName(request.Name);

        if (request.NameColor is not null)
        {
            var nameColor = Color.FromHex(request.NameColor);
            
            character.UpdateNameColor(nameColor);
        }

        if (request.Description is not null)
            character.UpdateDescription(request.Description);

        await unitOfWork.SaveChangesAsync(ct);
        
        return mapper.ToDto(character);
    }
}