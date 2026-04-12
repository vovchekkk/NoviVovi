using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddShowMenuStepCommand : AddStepCommand
{
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required string Text { get; init; }
    public required IEnumerable<ChoiceDto> Choices { get; init; }
}

public class AddShowMenuStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddShowMenuStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowMenuStepCommand request, CancellationToken ct)
    {
        var (_, label) = await GetStepContextOrThrow(request, ct);

        var targetIds = request.Choices.Select(c => c.Transition.TargetLabelId).Distinct();
        var targetLabels = await LabelRepository.GetByIdsAsync(targetIds, ct);
        
        var labelLookup = targetLabels.ToDictionary(l => l.Id);
        
        var menu = Domain.Menu.Menu.Create(request.Name, request.Description, request.Text);
        
        foreach (var choiceDto in request.Choices)
        {
            if (!labelLookup.TryGetValue(choiceDto.Transition.TargetLabelId, out var targetLabel))
                throw new NotFoundException($"Целевая метка {choiceDto.Transition.TargetLabelId} не найдена");

            var choice = Choice.Create(
                ChoiceTransition.Create(targetLabel),
                choiceDto.Name,
                choiceDto.Description,
                choiceDto.Text
            );
            
            menu.AddChoice(choice);
        }
        
        var step = ShowMenuStep.Create(menu);

        label.AddStep(step);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}