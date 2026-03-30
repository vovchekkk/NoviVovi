using MediatR;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Labels.Features.Add;

public record AddLabelCommand : IRequest<LabelDto>
{
    public required Guid NovelId { get; init; }
    public required string Name { get; init; }
}

public class AddLabelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    LabelDtoMapper mapper
) : IRequestHandler<AddLabelCommand, LabelDto>
{
    public Task<LabelDto> Handle(AddLabelCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}