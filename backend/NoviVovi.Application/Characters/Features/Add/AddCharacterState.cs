using MediatR;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Characters;

namespace NoviVovi.Application.Characters.Features.Add;

public record AddCharacterStateCommand : IRequest<CharacterStateDto>
{
    public required Guid NovelId { get; init; }
    public required Guid CharacterId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Guid ImageId { get; init; }
    public required TransformDto LocalTransform { get; init; }
}

public class AddCharacterStateHandler(
    INovelRepository novelRepository,
    IImageRepository imageRepository,
    TransformDtoMapper transformMapper,
    IUnitOfWork unitOfWork,
    CharacterStateDtoMapper mapper
) : IRequestHandler<AddCharacterStateCommand, CharacterStateDto>
{
    public async Task<CharacterStateDto> Handle(AddCharacterStateCommand request, CancellationToken ct)
    {
        var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);
        var character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

        var image = await imageRepository.GetByIdAsync(request.ImageId, ct);

        var transform = transformMapper.ToDomainModel(request.LocalTransform);

        var state = CharacterState.Create(request.Name, image, transform, request.Description);

        character.AddCharacterState(state);
        
        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(state);
    }
}