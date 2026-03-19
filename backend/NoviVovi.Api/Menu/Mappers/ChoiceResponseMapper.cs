using NoviVovi.Api.Menu.Responses;
using NoviVovi.Application.Menu.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Menu.Mappers;

[Mapper]
public partial class ChoiceResponseMapper
{
    public partial ChoiceResponse ToResponse(ChoiceSnapshot novel);

    public partial IEnumerable<ChoiceResponse> ToResponses(IEnumerable<ChoiceSnapshot> novels);
}