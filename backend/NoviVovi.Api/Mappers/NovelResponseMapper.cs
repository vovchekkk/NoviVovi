using NoviVovi.Api.Contracts.Novels.Responses;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.DTO;

namespace NoviVovi.Api.Mappers;

public static class NovelResponseMapper
{
    public static NovelResponse ToResponse(this NovelDto dto)
    {
        return new NovelResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            Slides = dto.Slides
                .Select(s => new SlideResponse
                {
                    Number = s.Number,
                    Text = s.Text
                })
                .ToList()
        };
    }
}