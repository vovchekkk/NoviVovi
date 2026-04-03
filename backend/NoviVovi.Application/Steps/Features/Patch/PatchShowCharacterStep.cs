using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
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
    StepDtoMapper mapper
) : BasePatchStepHandler(novelRepository, labelRepository), IRequestHandler<PatchShowCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchShowCharacterStepCommand request, CancellationToken ct)
    {
        var (_, label, step) = await GetStepContextOrThrow(request, ct);

        if (step is not ShowCharacterStep showCharacterStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(ShowCharacterStep)}");

        // await labelRepository.SaveAsync(label, ct);
        return mapper.ToDto(showCharacterStep);
    }
}