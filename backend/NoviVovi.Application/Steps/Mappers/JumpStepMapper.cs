using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class JumpStepMapper
{
    public partial JumpStepSnapshot ToSnapshot(JumpStep novel);
    
    public partial IEnumerable<JumpStepSnapshot> ToSnapshots(IEnumerable<JumpStep> novels);
}