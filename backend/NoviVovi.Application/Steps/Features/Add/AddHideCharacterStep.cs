using MediatR;
using NoviVovi.Application.Characters.Features.Get;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddHideCharacterStepCommand : AddStepCommand
{
    public required Guid CharacterId { get; init; }
}

public class AddHideCharacterStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BaseAddStepHandler(novelRepository, labelRepository), IRequestHandler<AddHideCharacterStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddHideCharacterStepCommand request, CancellationToken ct)
    {
        var (_, label) = await GetStepContextOrThrow(request, ct);

        var character = await novelRepository.GetCharacterByIdAsync(request.CharacterId, ct)
                        ?? throw new NotFoundException($"Персонаж '{request.CharacterId}' не найден");

        var step = HideCharacterStep.Create(character);

        label.AddStep(step);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}