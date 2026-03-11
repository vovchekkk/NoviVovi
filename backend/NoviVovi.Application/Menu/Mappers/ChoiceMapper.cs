using NoviVovi.Application.Menu.Contracts;
using NoviVovi.Domain.Menu;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Menu.Mappers;

[Mapper]
public partial class ChoiceMapper
{
    public partial ChoiceSnapshot ToSnapshot(Choice novel);

    public partial IEnumerable<ChoiceSnapshot> ToSnapshots(IEnumerable<Choice> novels);
}