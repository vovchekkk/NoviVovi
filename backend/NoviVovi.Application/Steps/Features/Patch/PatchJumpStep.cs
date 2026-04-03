using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchJumpStepCommand : PatchStepCommand
{
    public Guid? TargetLabelId { get; init; }
}

public class PatchJumpStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BasePatchStepHandler(novelRepository, labelRepository), IRequestHandler<PatchJumpStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchJumpStepCommand request, CancellationToken ct)
    {
        var (_, label, step) = await GetStepContextOrThrow(request, ct);

        if (step is not JumpStep jumpStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(JumpStep)}");

        // await labelRepository.SaveAsync(label, ct);
        return mapper.ToDto(jumpStep);
    }
}