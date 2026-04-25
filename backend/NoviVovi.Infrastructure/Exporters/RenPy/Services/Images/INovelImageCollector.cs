using NoviVovi.Domain.Novels;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;

/// <summary>
/// Collects all unique images from a novel with RenPy naming information.
/// Single Responsibility: Image collection logic.
/// </summary>
public interface INovelImageCollector
{
    IEnumerable<ImageExportInfo> CollectImages(Novel novel);
}
