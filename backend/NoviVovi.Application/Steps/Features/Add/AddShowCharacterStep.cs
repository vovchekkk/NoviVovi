using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddShowCharacterStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
    public required Guid CharacterStateId { get; init; }
    public required TransformDto Transform { get; init; }
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