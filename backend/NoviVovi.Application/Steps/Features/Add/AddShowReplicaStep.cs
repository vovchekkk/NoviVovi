using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddShowReplicaStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
    public required string Text { get; init; }
}

public class AddShowReplicaStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BaseAddStepHandler(labelRepository), IRequestHandler<AddShowReplicaStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowReplicaStepCommand request, CancellationToken ct)
    {
        var label = await GetStepContextOrThrow(request, ct);

        var character = await novelRepository.GetCharacterByIdAsync(request.NovelId, request.CharacterId, ct)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
        
        var replica = Replica.Create(character, request.Text);

        var step = ShowReplicaStep.Create(replica);

        label.AddStep(step);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}