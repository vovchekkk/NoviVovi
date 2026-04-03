using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddHideCharacterStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
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