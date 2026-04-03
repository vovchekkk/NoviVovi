using NoviVovi.Api.Labels.Requests;
using NoviVovi.Application.Labels.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Labels.CommandMappers;

[Mapper]
public partial class PatchLabelCommandMapper
{
    public partial PatchLabelCommand ToCommand(PatchLabelRequest request, Guid novelId, Guid labelId);
}