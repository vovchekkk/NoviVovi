using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowMenuStepSnapshotMapper
{
    public partial ShowMenuStepSnapshot ToSnapshot(ShowMenuStep subject);
    
    public partial IEnumerable<ShowMenuStepSnapshot> ToSnapshots(IEnumerable<ShowMenuStep> subjects);
}