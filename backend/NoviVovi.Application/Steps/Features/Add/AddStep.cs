using MediatR;
using NoviVovi.Application.Common.Dtos;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Steps.Features.Add;

public abstract record AddStepCommand : IRequest<StepDto>
{
    public required Guid NovelId { get; init; }
    public required Guid LabelId { get; init; }
}

public record AddHideCharacterStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
}

public record AddJumpStepCommand : AddStepCommand
{
    public required Guid TargetLabelId { get; init; }
}

public record AddShowBackgroundStepCommand : AddStepCommand
{
    public required Guid ImageId { get; init; }
    public required TransformPatchDto Transform { get; init; }
}

public record AddShowCharacterStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
    public required Guid CharacterStateId { get; init; }
    public required TransformPatchDto Transform { get; init; }
}

public record AddShowMenuStepCommand : AddStepCommand
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Text { get; init; }
}

public record AddShowReplicaStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
    public required string Text { get; init; }
}

public abstract class BaseAddStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository)
{
    protected async Task<(Novel, Label)> GetStepContextOrThrow(AddStepCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var label = await labelRepository.GetByIdAsync(request.LabelId)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");

        return (novel, label);
    }
}

public class AddHideCharacterStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddHideCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddHideCharacterStepCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
        
        var (_, label) = await GetStepContextOrThrow(request, ct);

        // await labelRepository.SaveAsync(label, ct);
        // return mapper.ToDto(hideCharacterStep);
    }
}

public class AddJumpStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddJumpStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddJumpStepCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
        
        var (_, label) = await GetStepContextOrThrow(request, ct);

        // await labelRepository.SaveAsync(label, ct);
        // return mapper.ToDto(jumpStep);
    }
}

public class AddShowBackgroundStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddShowBackgroundStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowBackgroundStepCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
        
        var (_, label) = await GetStepContextOrThrow(request, ct);

        // await labelRepository.SaveAsync(label, ct);
        // return mapper.ToDto(showBackgroundStep);
    }
}

public class AddShowCharacterStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddShowCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowCharacterStepCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
        
        var (_, label) = await GetStepContextOrThrow(request, ct);

        // await labelRepository.SaveAsync(label, ct);
        // return mapper.ToDto(showCharacterStep);
    }
}

public class AddShowMenuStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddShowMenuStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowMenuStepCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
        
        var (_, label) = await GetStepContextOrThrow(request, ct);

        // await labelRepository.SaveAsync(label, ct);
        // return mapper.ToDto(showMenuStep);
    }
}

public class AddShowReplicaStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddShowReplicaStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowReplicaStepCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
        
        var (_, label) = await GetStepContextOrThrow(request, ct);

        // await labelRepository.SaveAsync(label, ct);
        // return mapper.ToDto(showReplicaStep);
    }
}