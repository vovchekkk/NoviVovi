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
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchShowCharacterStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
    public Guid? CharacterStateId { get; init; }
    public TransformDto? Transform { get; init; }
}

public class PatchShowCharacterStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    TransformDtoMapper transformMapper,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(labelRepository), IRequestHandler<PatchShowCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchShowCharacterStepCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var (label, step) = await GetStepContextOrThrow(request, ct);
            
            var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);

            if (step is not ShowCharacterStep showCharacterStep)
                throw new BadRequestException($"Step {step.Id} is not {typeof(ShowCharacterStep)}");
            
            Character? character = null;
            if (request.CharacterId.HasValue)
            {
                character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
            }
            
            CharacterState? state = null;
            if (request.CharacterId.HasValue && request.CharacterStateId.HasValue)
            {
                character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
                
                state = character.CharacterStates.FirstOrDefault(c => c.Id == request.CharacterStateId)
                                    ?? throw new NotFoundException($"Состояние персонажа '{request.CharacterStateId}' не найдено");
            }

            var transformPatch = request.Transform != null 
                ? transformMapper.ToDomainPatch(request.Transform) 
                : null;
            
            showCharacterStep.Update(character, state, transformPatch);
            
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