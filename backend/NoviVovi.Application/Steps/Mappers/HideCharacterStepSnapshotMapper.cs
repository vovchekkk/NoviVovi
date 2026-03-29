using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class HideCharacterStepSnapshotMapper
{
    public partial HideCharacterStepSnapshot ToSnapshot(HideCharacterStep subject);
    
    public partial IEnumerable<HideCharacterStepSnapshot> ToSnapshots(IEnumerable<HideCharacterStep> subjects);
}