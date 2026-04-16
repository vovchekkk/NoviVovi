using NoviVovi.Application.Menu.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Menu.Mappers;

[Mapper]
public partial class MenuResponseMapper
{
    public partial MenuDto ToResponse(Domain.Menu.Menu source);

    public partial IEnumerable<MenuDto> ToResponses(IEnumerable<Domain.Menu.Menu> sources);
}