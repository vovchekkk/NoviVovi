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

namespace NoviVovi.Application.Steps.Features.Patch;

public record PatchShowMenuStepCommand : PatchStepCommand
{
    public required IEnumerable<ChoiceDto>? Choices { get; init; }
}

public class PatchShowMenuStepHandler(
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BasePatchStepHandler(labelRepository), IRequestHandler<PatchShowMenuStepCommand, StepDto>
{
    private readonly ILabelRepository _labelRepository = labelRepository;

    public async Task<StepDto> Handle(PatchShowMenuStepCommand request, CancellationToken ct)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var (label, step) = await GetStepContextOrThrow(request, ct);

            if (step is not ShowMenuStep showMenuStep)
                throw new BadRequestException($"Step {step.Id} is not {typeof(ShowMenuStep)}");

            Domain.Menu.Menu? menu = null;
            if (request.Choices is not null)
            {
                var targetIds = request.Choices.Select(c => c.Transition.TargetLabelId).Distinct();
                var targetLabels = await _labelRepository.GetByIdsAsync(targetIds, ct);

                var labelLookup = targetLabels.ToDictionary(l => l.Id);

                menu = Domain.Menu.Menu.Create();

                foreach (var choiceDto in request.Choices)
                {
                    if (!labelLookup.TryGetValue(choiceDto.Transition.TargetLabelId, out var targetLabel))
                        throw new NotFoundException($"Целевая метка {choiceDto.Transition.TargetLabelId} не найдена");

                    var choice = Choice.Create(
                        choiceDto.Text,
                        ChoiceTransition.Create(targetLabel)
                    );

                    menu.AddChoice(choice);
                }
            }

            showMenuStep.Update(menu);
            
            await labelRepository.AddOrUpdateAsync(label, ct);

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