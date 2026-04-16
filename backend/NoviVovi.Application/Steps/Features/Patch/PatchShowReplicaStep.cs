using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
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
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(labelRepository), IRequestHandler<PatchShowReplicaStepCommand, StepDto>
{
    public async Task<StepDto> Handle(PatchShowReplicaStepCommand request, CancellationToken ct)
    {
        var (_, step) = await GetStepContextOrThrow(request, ct);

        if (step is not ShowReplicaStep showReplicaStep)
            throw new BadRequestException($"Step {step.Id} is not {typeof(ShowReplicaStep)}");

        Character? character = null;
        if (request.CharacterId.HasValue)
        {
            character = await novelRepository.GetCharacterByIdAsync(request.NovelId, request.CharacterId.Value, ct)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        }

        var replica = Replica.Create(character, request.Text);

        showReplicaStep.Update(replica);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}