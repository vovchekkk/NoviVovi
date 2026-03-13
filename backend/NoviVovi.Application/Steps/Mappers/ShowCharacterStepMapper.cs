using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class ShowCharacterStepMapper
{
    public partial ShowCharacterStepSnapshot ToSnapshot(ShowCharacterStep novel);
    
    public partial IEnumerable<ShowCharacterStepSnapshot> ToSnapshots(IEnumerable<ShowCharacterStep> novels);
}