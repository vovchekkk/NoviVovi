using NoviVovi.Api.Labels.Requests.Add;
using NoviVovi.Application.Labels.Features.Add;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Labels.CommandMappers;

[Mapper]
public partial class AddCommandMapper
{
    public partial AddLabelCommand ToCommand(AddLabelRequest request, Guid novelId);
}