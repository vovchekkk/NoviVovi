using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Contracts.Novels;

public static class NovelDtoMapper
{
    public static NovelSnapshot ToDto(this Novel novel)
    {
        return new NovelSnapshot
        {
            Id = novel.Id,
            Title = novel.Title
        };
    }
}