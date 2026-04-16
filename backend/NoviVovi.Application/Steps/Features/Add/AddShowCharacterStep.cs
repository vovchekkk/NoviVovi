using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddShowCharacterStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
    public required Guid CharacterStateId { get; init; }
    public required TransformDto Transform { get; init; }
}

public class AddShowCharacterStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    TransformDtoMapper transformMapper,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BaseAddStepHandler(labelRepository), IRequestHandler<AddShowCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowCharacterStepCommand request, CancellationToken ct)
    {
        var label = await GetStepContextOrThrow(request, ct);

        var character = await novelRepository.GetCharacterByIdAsync(request.NovelId, request.CharacterId, ct)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        
        var characterState = await novelRepository.GetCharacterStateByIdAsync(request.NovelId, request.CharacterId, request.CharacterStateId, ct)
                            ?? throw new NotFoundException($"Состояние персонажа '{request.CharacterStateId}' не найдено");

        var transform = transformMapper.ToDomainModel(request.Transform);
        
        var characterObject = CharacterObject.Create(character, characterState, transform);

        var step = ShowCharacterStep.Create(characterObject);

        label.AddStep(step);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}