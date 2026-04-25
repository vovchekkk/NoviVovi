namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;

/// <summary>
/// Contains information about an image to export to RenPy.
/// </summary>
public record ImageExportInfo(
    Guid ImageId,
    string RenPyImageName
);
