using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchShowMenuStepCommand : PatchStepCommand
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Text { get; init; }
    public MenuDto? Menu { get; init; }
}

public class PatchShowMenuStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BasePatchStepHandler(novelRepository, labelRepository), IRequestHandler<PatchShowMenuStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchShowMenuStepCommand request, CancellationToken ct)
    {
        var (_, label, step) = await GetStepContextOrThrow(request, ct);

        if (step is not ShowMenuStep showMenuStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(ShowMenuStep)}");

        // await labelRepository.SaveAsync(label, ct);
        return mapper.ToDto(showMenuStep);
    }
}