using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchShowReplicaStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
    public string? Text { get; init; }
}

public class PatchShowReplicaStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BasePatchStepHandler(novelRepository, labelRepository), IRequestHandler<PatchShowReplicaStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchShowReplicaStepCommand request, CancellationToken ct)
    {
        var (_, label, step) = await GetStepContextOrThrow(request, ct);

        if (step is not ShowReplicaStep showReplicaStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(ShowReplicaStep)}");

        // await labelRepository.SaveAsync(label, ct);
        return mapper.ToDto(showReplicaStep);
    }
}