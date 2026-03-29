using NoviVovi.Api.Labels.Responses;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Domain.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Labels.Mappers;

[Mapper]
public partial class LabelResponseMapper
{
    public partial LabelResponse ToResponse(LabelDto subject);

    public partial IEnumerable<LabelResponse> ToResponses(IEnumerable<LabelDto> subjects);
}