using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddShowReplicaStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
    public required string Text { get; init; }
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