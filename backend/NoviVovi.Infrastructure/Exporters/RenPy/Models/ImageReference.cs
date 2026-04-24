namespace NoviVovi.Infrastructure.Exporters.RenPy.Models;

internal class ImageReference
{
    public Guid Id { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public ImageType Type { get; set; }
    public Guid? CharacterId { get; set; }
    public string? StateName { get; set; }
}