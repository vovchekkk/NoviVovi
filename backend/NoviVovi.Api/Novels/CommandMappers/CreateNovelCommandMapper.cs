using NoviVovi.Api.Novels.Requests;
using NoviVovi.Application.Novels.Features.Create;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.CommandMappers;

[Mapper]
public partial class CreateNovelCommandMapper
{
    public partial CreateNovelCommand ToCommand(CreateNovelRequest request);
}