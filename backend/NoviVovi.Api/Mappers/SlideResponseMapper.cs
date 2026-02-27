using NoviVovi.Api.Contracts.Novels.Responses;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.DTO;

namespace NoviVovi.Api.Mappers;

public static class SlideResponseMapper
{
    public static SlideResponse ToResponse(this SlideDto dto)
    {
        return new SlideResponse
        {
            Number = dto.Number,
            Text = dto.Text
        };
    }
}