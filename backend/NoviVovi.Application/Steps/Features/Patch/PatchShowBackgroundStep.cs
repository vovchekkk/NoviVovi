using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchShowBackgroundStepCommand : PatchStepCommand
{
    public Guid? ImageId { get; init; }
    public TransformDto? Transform { get; init; }
}

public class PatchShowBackgroundStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BasePatchStepHandler(novelRepository, labelRepository), IRequestHandler<PatchShowBackgroundStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchShowBackgroundStepCommand request, CancellationToken ct)
    {
        var (_, label, step) = await GetStepContextOrThrow(request, ct);

        if (step is not ShowBackgroundStep showBackgroundStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(ShowBackgroundStep)}");

        // await labelRepository.SaveAsync(label, ct);
        return mapper.ToDto(showBackgroundStep);
    }
}