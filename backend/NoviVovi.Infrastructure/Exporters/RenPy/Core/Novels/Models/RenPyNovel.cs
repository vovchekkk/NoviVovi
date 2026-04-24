using NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Models;

/// <summary>
/// Represents a complete Ren'Py project ready for export.
/// </summary>
public class RenPyNovel
{
    public required string Title { get; init; }
    public required List<RenPyCharacter> Characters { get; init; }
    public required List<RenPyLabel> Labels { get; init; }
    public required string StartLabelId { get; init; }
}
