using MediatR;
using NoviVovi.Application.Novels.Contracts;

namespace NoviVovi.Application.Novels.Features.Get;

public record GetNovelsQuery : IRequest<IEnumerable<NovelSnapshot>>
{
    
}

public class GetNovelsHandler
{
    
}