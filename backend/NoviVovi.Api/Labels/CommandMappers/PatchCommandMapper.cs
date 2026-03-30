using NoviVovi.Api.Labels.Requests.Patch;
using NoviVovi.Application.Labels.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Labels.CommandMappers;

[Mapper]
public partial class PatchCommandMapper
{
    public partial PatchLabelCommand ToCommand(PatchLabelRequest request, Guid novelId, Guid labelId);
}