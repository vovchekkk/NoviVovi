using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
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
        unitOfWork.BeginTransaction();
        
        try
        {
            var step = await GetStepContextOrThrow(request, ct);
            
            var allCharacters = await novelRepository.GetAllCharactersAsync(request.NovelId, ct);

            if (step is not ShowReplicaStep showReplicaStep)
                throw new BadRequestException($"Step {step.Id} is not {typeof(ShowReplicaStep)}");

            Character? character = null;
            if (request.CharacterId.HasValue)
            {
                character = allCharacters.FirstOrDefault(c => c.Id == request.CharacterId)
                            ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");
            }

            var replica = Replica.Create(character, request.Text);

            showReplicaStep.Update(replica);

            await unitOfWork.CommitAsync(ct);

            return mapper.ToDto(step);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}