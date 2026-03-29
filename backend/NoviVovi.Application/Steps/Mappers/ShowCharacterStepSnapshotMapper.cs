using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowCharacterStepSnapshotMapper
{
    public partial ShowCharacterStepSnapshot ToSnapshot(ShowCharacterStep subject);
    
    public partial IEnumerable<ShowCharacterStepSnapshot> ToSnapshots(IEnumerable<ShowCharacterStep> subjects);
}