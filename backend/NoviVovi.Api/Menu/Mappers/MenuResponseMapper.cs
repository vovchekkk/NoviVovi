using NoviVovi.Api.Menu.Responses;
using NoviVovi.Application.Menu.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Menu.Mappers;

[Mapper]
public partial class MenuResponseMapper(
    ChoiceResponseMapper choiceMapper
)
{
    public MenuResponse ToResponse(MenuDto source) => new(
        Choices: choiceMapper.ToResponses(source.Choices).ToList()
    );

    public IEnumerable<MenuResponse> ToResponses(IEnumerable<MenuDto> sources) =>
        sources.Select(ToResponse);
}
