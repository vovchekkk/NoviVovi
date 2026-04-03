using NoviVovi.Api.Images.Requests;
using NoviVovi.Application.Images.Features.Upload;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.CommandMappers;

[Mapper]
public partial class UploadImageCommandMapper
{
    public partial UploadImageCommand ToCommand(UploadImageRequest request);
}