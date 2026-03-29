using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowBackgroundStepSnapshotMapper
{
    public partial ShowBackgroundStepSnapshot ToSnapshot(ShowBackgroundStep subject);
    
    public partial IEnumerable<ShowBackgroundStepSnapshot> ToSnapshots(IEnumerable<ShowBackgroundStep> subjects);
}