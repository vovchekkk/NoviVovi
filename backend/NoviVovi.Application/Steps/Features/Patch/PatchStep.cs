using MediatR;
using NoviVovi.Application.Common.Dtos;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public abstract record PatchStepCommand : IRequest<StepDto>
{
    public required Guid NovelId { get; init; }
    public required Guid LabelId { get; init; }
    public required Guid StepId { get; init; }
}

public record PatchHideCharacterStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
}

public record PatchJumpStepCommand : PatchStepCommand
{
    public Guid? TargetLabelId { get; init; }
}

public record PatchShowBackgroundStepCommand : PatchStepCommand
{
    public Guid? ImageId { get; init; }
    public TransformPatchDto? Transform { get; init; }
}

public record PatchShowCharacterStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
    public Guid? CharacterStateId { get; init; }
    public TransformPatchDto? Transform { get; init; }
}

public record PatchShowMenuStepCommand : PatchStepCommand
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Text { get; init; }
}

public record PatchShowReplicaStepCommand : PatchStepCommand
{
    public Guid? CharacterId { get; init; }
    public string? Text { get; init; }
}

public abstract class BasePatchStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository)
{
    protected async Task<(Novel, Label, Step)> GetStepContextOrThrow(PatchStepCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var label = await labelRepository.GetByIdAsync(request.LabelId)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");

        var step = label.Steps.FirstOrDefault(s => s.Id == request.StepId)
                   ?? throw new NotFoundException($"Шаг '{request.StepId}' не найден в метке");

        return (novel, label, step);
    }
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