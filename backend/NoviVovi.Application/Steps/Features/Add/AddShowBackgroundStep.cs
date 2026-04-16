using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Images;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Add;

public record AddShowBackgroundStepCommand : AddStepCommand
{
    public required Guid ImageId { get; init; }
    public required TransformDto Transform { get; init; }
}

public class AddShowBackgroundStepHandler(
    ILabelRepository labelRepository,
    IImageRepository imageRepository,
    TransformDtoMapper transformMapper,
    IUnitOfWork unitOfWork,
    StepDtoMapper mapper
) : BaseAddStepHandler(labelRepository), IRequestHandler<AddShowBackgroundStepCommand, StepDto>
{
    public async Task<StepDto> Handle(AddShowBackgroundStepCommand request, CancellationToken ct)
    {
        var label = await GetStepContextOrThrow(request, ct);
        
        var image = await imageRepository.GetByIdAsync(request.ImageId, ct)
                    ?? throw new NotFoundException($"Изображение '{request.ImageId}' не найдено");
        
        var transform = transformMapper.ToDomainModel(request.Transform);
        
        var background = BackgroundObject.Create(image, transform);
        
        var step = ShowBackgroundStep.Create(background);

        label.AddStep(step);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.ToDto(step);
    }
}