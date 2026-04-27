using MediatR;
using NoviVovi.Application.Characters.Abstactions;
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
using NoviVovi.Domain.Images;

namespace NoviVovi.Application.Characters.Features.Patch;

public record PatchCharacterStateCommand : IRequest<CharacterStateDto>
{
    public required Guid NovelId { get; init; }
    public required Guid CharacterId { get; init; }
    public required Guid StateId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public Guid? ImageId { get; init; }
    public TransformDto? LocalTransform { get; init; }
}

public class PatchCharacterStateHandler(
    INovelRepository novelRepository,
    ICharacterRepository characterRepository,
    IImageRepository imageRepository,
    TransformDtoMapper transformMapper,
    IUnitOfWork unitOfWork,
    CharacterStateDtoMapper mapper
) : IRequestHandler<PatchCharacterStateCommand, CharacterStateDto>
{
    public async Task<CharacterStateDto> Handle(PatchCharacterStateCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var character = await characterRepository.GetByIdAsync(request.CharacterId, ct)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

            var state = character.CharacterStates.FirstOrDefault(c => c.Id == request.StateId)
                        ?? throw new NotFoundException($"Состояние персонажа '{request.StateId}' не найдено");
            
            if (request.Name is not null)
                state.UpdateName(request.Name);
            
            if (request.Description is not null)
                state.UpdateDescription(request.Description);
            
            if (request.ImageId.HasValue)
            {
                var image = await imageRepository.GetByIdAsync(request.ImageId.Value, ct)
                        ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
                
                state.UpdateImage(image);
            }

            if (request.LocalTransform is not null)
            {
                var transformPatch = transformMapper.ToDomainPatch(request.LocalTransform);
                
                state.PatchTransform(transformPatch);
            }
            
            await characterRepository.AddOrUpdateAsync(character, ct);

            await unitOfWork.CommitAsync(ct);

            return mapper.ToDto(state);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}