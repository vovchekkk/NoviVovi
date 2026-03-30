using NoviVovi.Api.Novels.Requests.Patch;
using NoviVovi.Application.Novels.Features.Patch;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.CommandMappers;

[Mapper]
public partial class PatchCommandMapper
{
    public partial PatchNovelCommand ToCommand(PatchNovelRequest request, Guid novelId);
}