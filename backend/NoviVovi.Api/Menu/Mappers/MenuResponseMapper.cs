using NoviVovi.Application.Menu.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Menu.Mappers;

[Mapper]
public partial class MenuResponseMapper
{
    public partial MenuSnapshot ToSnapshot(Domain.Menu.Menu novel);

    public partial IEnumerable<MenuSnapshot> ToSnapshots(IEnumerable<Domain.Menu.Menu> novels);
}