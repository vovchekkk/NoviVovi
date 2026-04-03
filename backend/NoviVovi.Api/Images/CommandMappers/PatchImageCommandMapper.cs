using NoviVovi.Api.Images.Requests;
using NoviVovi.Application.Images.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.CommandMappers;

[Mapper]
public partial class PatchImageCommandMapper
{
    public partial PatchImageCommand ToCommand(PatchImageRequest request, Guid imageId);
}