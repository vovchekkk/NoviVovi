using NoviVovi.Domain.Novels;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;

/// <summary>
/// Collects all unique image IDs from a novel.
/// Single Responsibility: Image collection logic.
/// </summary>
public interface INovelImageCollector
{
    IEnumerable<Guid> CollectImageIds(Novel novel);
}
