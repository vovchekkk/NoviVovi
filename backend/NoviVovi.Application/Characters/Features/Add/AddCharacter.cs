using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels;
using NoviVovi.Domain.Characters;

namespace NoviVovi.Application.Characters.Features.Add;

public record AddCharacterCommand : IRequest<CharacterDto>
{
    public required Guid NovelId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public class AddCharacterHandler(
    INovelRepository novelRepository,
    IUnitOfWork unitOfWork,
    CharacterDtoMapper mapper
) : IRequestHandler<AddCharacterCommand, CharacterDto>
{
    public async Task<CharacterDto> Handle(AddCharacterCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var character = Character.Create(request.Name, request.Description);

        novel.AddCharacter(character);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(character);
    }
}