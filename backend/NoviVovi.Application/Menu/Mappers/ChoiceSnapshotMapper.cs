using NoviVovi.Application.Menu.Contracts;
using NoviVovi.Domain.Menu;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Menu.Mappers;

[Mapper]
public partial class ChoiceSnapshotMapper
{
    public partial ChoiceSnapshot ToSnapshot(Choice subject);

    public partial IEnumerable<ChoiceSnapshot> ToSnapshots(IEnumerable<Choice> subjects);
}