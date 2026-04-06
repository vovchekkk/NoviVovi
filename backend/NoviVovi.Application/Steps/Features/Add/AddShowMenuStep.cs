using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddShowMenuStepCommand : AddStepCommand
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Text { get; init; }
    public required MenuDto Menu { get; init; }
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