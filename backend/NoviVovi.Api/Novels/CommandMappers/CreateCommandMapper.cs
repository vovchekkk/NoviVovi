using NoviVovi.Api.Novels.Requests.Create;
using NoviVovi.Application.Novels.Features.Create;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.CommandMappers;

[Mapper]
public partial class CreateCommandMapper
{
    public partial CreateNovelCommand ToCommand(CreateNovelRequest request);
}