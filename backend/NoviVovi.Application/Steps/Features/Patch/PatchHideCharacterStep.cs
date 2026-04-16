using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchHideCharacterStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
}

public class PatchHideCharacterStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(labelRepository), IRequestHandler<PatchHideCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchHideCharacterStepCommand request, CancellationToken ct)
    {
        var (_, step) = await GetStepContextOrThrow(request, ct);

        if (step is not HideCharacterStep hideCharacterStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(HideCharacterStep)}");
        
        Character? character = null;
        if (request.CharacterId.HasValue)
        {
            character = await novelRepository.GetCharacterByIdAsync(request.NovelId, request.CharacterId.Value, ct)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        }
        
        hideCharacterStep.Update(character);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}