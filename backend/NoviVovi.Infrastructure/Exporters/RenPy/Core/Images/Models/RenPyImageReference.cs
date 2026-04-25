namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Images.Models;

internal class RenPyImageReference
{
    public Guid Id { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public RenPyImageType Type { get; set; }
    public Guid? CharacterId { get; set; }
    public string? StateName { get; set; }
}