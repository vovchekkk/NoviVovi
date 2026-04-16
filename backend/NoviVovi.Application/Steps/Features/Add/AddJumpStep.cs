using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddJumpStepCommand : AddStepCommand
{
    public required Guid TargetLabelId { get; init; }
}

public class AddJumpStepHandler(
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BaseAddStepHandler(labelRepository), IRequestHandler<AddJumpStepCommand, StepDto>
{
    private readonly ILabelRepository _labelRepository = labelRepository;

    public async Task<StepDto> Handle(AddJumpStepCommand request, CancellationToken ct)
    {
        var label = await GetStepContextOrThrow(request, ct);
        
        var targetLabel = await _labelRepository.GetByIdAsync(request.TargetLabelId, ct)
                            ?? throw new NotFoundException($"Метка '{request.TargetLabelId}' не найдена");
        
        var step = JumpStep.Create(targetLabel);

        label.AddStep(step);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}