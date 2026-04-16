namespace NoviVovi.Application.Images.Dtos;

public record UploadInfoImageDto(
    Guid ImageId, 
    string UploadUrl,
    string ViewUrl
);