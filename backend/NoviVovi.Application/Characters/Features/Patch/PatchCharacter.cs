using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Characters.Features.Patch;

public record PatchCharacterCommand : IRequest<CharacterDto>
{
    public required Guid NovelId { get; init; }
    public required Guid CharacterId { get; init; }
    public string? Name { get; init; }
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
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var character = novel.Characters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        
        if (request.Name is not null)
            character.UpdateName(request.Name);
        
        if (request.Description is not null)
            character.UpdateDescription(request.Description);

        await unitOfWork.SaveChangesAsync(ct);
        
        return mapper.ToDto(character);
    }
}