using NoviVovi.Api.Images.Requests;
using NoviVovi.Application.Images.Features.InitiateUpload;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.CommandMappers;

[Mapper]
public partial class InitiateUploadImageCommandMapper
{
    public partial InitiateUploadImageCommand ToCommand(InitiateUploadImageRequest request);
}