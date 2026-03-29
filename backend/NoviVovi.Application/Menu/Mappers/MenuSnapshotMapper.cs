using NoviVovi.Application.Menu.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Menu.Mappers;

[Mapper]
public partial class MenuSnapshotMapper
{
    public partial MenuSnapshot ToSnapshot(Domain.Menu.Menu subject);

    public partial IEnumerable<MenuSnapshot> ToSnapshots(IEnumerable<Domain.Menu.Menu> subjects);
}