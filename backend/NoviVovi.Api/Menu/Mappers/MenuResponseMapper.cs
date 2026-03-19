using NoviVovi.Application.Menu.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Menu.Mappers;

[Mapper]
public partial class MenuResponseMapper
{
    public partial MenuSnapshot ToResponse(Domain.Menu.Menu novel);

    public partial IEnumerable<MenuSnapshot> ToResponses(IEnumerable<Domain.Menu.Menu> novels);
}