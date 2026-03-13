using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class HideCharacterStepMapper
{
    public partial HideCharacterStepSnapshot ToSnapshot(HideCharacterStep novel);
    
    public partial IEnumerable<HideCharacterStepSnapshot> ToSnapshots(IEnumerable<HideCharacterStep> novels);
}