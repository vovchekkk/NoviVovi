using NoviVovi.Api.Images.Requests;
using NoviVovi.Application.Images.Features.InitiateUpload;
using NoviVovi.Application.Images.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.CommandMappers;

[Mapper]
public partial class ImageCommandMapper
{
    public partial PatchImageCommand ToCommand(PatchImageRequest request, Guid imageId);
    
    public partial InitiateUploadImageCommand ToCommand(InitiateUploadImageRequest request, Guid novelId);
}