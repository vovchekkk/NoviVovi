using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
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
        var (_, step) = await GetStepContextOrThrow(request, ct);

        if (step is not ShowCharacterStep showCharacterStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(ShowCharacterStep)}");
        
        Character? character = null;
        if (request.CharacterId.HasValue)
        {
            character = await novelRepository.GetCharacterByIdAsync(request.NovelId, request.CharacterId.Value, ct)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        }
        
        CharacterState? state = null;
        if (request.CharacterId.HasValue && request.CharacterStateId.HasValue)
        {
            state = await novelRepository.GetCharacterStateByIdAsync(request.NovelId, request.CharacterId.Value, request.CharacterStateId.Value, ct)
                    ?? throw new NotFoundException($"Состояние персонажа '{request.CharacterStateId}' не найдено");
        }

        var transformPatch = request.Transform != null 
            ? transformMapper.ToDomainPatch(request.Transform) 
            : null;
        
        showCharacterStep.Update(character, state, transformPatch);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}