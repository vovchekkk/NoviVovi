using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
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
    public required IEnumerable<ChoiceDto> Choices { get; init; }
}

public class AddShowMenuStepHandler(
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BaseAddStepHandler(labelRepository), IRequestHandler<AddShowMenuStepCommand, StepDto>
{
    private readonly ILabelRepository _labelRepository = labelRepository;

    public async Task<StepDto> Handle(AddShowMenuStepCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var label = await GetStepContextOrThrow(request, ct);

            var targetIds = request.Choices.Select(c => c.Transition.TargetLabelId).Distinct();
            var targetLabels = await _labelRepository.GetByIdsAsync(targetIds, ct);
            
            var labelLookup = targetLabels.ToDictionary(l => l.Id);
            
            var menu = Domain.Menu.Menu.Create();
            
            foreach (var choiceDto in request.Choices)
            {
                if (!labelLookup.TryGetValue(choiceDto.Transition.TargetLabelId, out var targetLabel))
                    throw new NotFoundException($"Целевая метка {choiceDto.Transition.TargetLabelId} не найдена");

                var transition = ChoiceTransition.Create(targetLabel);
                var choice = Choice.Create(choiceDto.Text, transition);
                menu.AddChoice(choice);
            }

            var step = ShowMenuStep.Create(menu);

            label.AddStep(step);

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