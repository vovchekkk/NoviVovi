namespace NoviVovi.Api.Images.Responses;

public record UploadInfoImageResponse(
    Guid ImageId, 
    string UploadUrl,
    string ViewUrl
);