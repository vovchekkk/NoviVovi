using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
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
        unitOfWork.BeginTransaction();
        
        try
        {
            var label = await GetStepContextOrThrow(request, ct);

            var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);
            var character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
            
            var state = character.CharacterStates.FirstOrDefault(c => c.Id == request.CharacterStateId)
                        ?? throw new NotFoundException($"Состояние персонажа '{request.CharacterId}' не найдено");

            var transform = transformMapper.ToDomainModel(request.Transform);
            
            var characterObject = CharacterObject.Create(character, state, transform);

            var step = ShowCharacterStep.Create(characterObject);

            label.AddStep(step);
            
            await labelRepository.AddOrUpdateAsync(label, ct);

            await unitOfWork.CommitAsync(ct);

            return mapper.ToDto(step);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}