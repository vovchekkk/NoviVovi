using NoviVovi.Api.Menu.Responses;
using NoviVovi.Application.Menu.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Menu.Mappers;

[Mapper]
public partial class ChoiceResponseMapper
{
    public partial ChoiceResponse ToResponse(ChoiceDto subject);

    public partial IEnumerable<ChoiceResponse> ToResponses(IEnumerable<ChoiceDto> subjects);
}