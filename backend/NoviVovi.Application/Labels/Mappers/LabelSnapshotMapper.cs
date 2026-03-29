using NoviVovi.Application.Labels.Contracts;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Domain.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Labels.Mappers;

[Mapper]
public partial class LabelSnapshotMapper
{
    public partial LabelSnapshot ToSnapshot(Label subject);

    public partial IEnumerable<LabelSnapshot> ToSnapshots(IEnumerable<Label> subjects);
}