using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddJumpStepCommand : AddStepCommand
{
    public required Guid TargetLabelId { get; init; }
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