using MediatR;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Add;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Labels.Features.Delete;

public record DeleteLabelCommand(
    Guid NovelId,
    Guid LabelId
) : IRequest;

public class DeleteLabelHandler(
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