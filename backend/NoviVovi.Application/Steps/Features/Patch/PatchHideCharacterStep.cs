using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchHideCharacterStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
}

public class PatchHideCharacterStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BasePatchStepHandler(novelRepository, labelRepository), IRequestHandler<PatchHideCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchHideCharacterStepCommand request, CancellationToken ct)
    {
        var (_, label, step) = await GetStepContextOrThrow(request, ct);

        if (step is not HideCharacterStep hideCharacterStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(HideCharacterStep)}");

        // await labelRepository.SaveAsync(label, ct);
        return mapper.ToDto(hideCharacterStep);
    }
}