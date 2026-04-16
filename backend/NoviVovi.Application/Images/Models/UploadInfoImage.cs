namespace NoviVovi.Application.Images.Models;

public class UploadInfoImage
{
    public required Guid ImageId { get; init; }
    public required string UploadUrl { get; init; }
    public required string ViewUrl { get; init; }
}