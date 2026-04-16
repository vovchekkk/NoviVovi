using NoviVovi.Application.Menu.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Menu.Mappers;

[Mapper]
public partial class MenuDtoMapper
{
    public partial MenuDto ToDto(Domain.Menu.Menu source);

    public partial IEnumerable<MenuDto> ToDtos(IEnumerable<Domain.Menu.Menu> sources);
}