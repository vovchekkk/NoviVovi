using NoviVovi.Api.Contracts.Novels.Responses;
using NoviVovi.Application.Novels.Contracts;

namespace NoviVovi.Api.Mappers;

public static class NovelResponseMapper
{
    public static NovelResponse ToResponse(this NovelSnapshot snapshot)
    {
        return new NovelResponse
        {
            Id = snapshot.Id,
            Title = snapshot.Title
        };
    }
}