using NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Script;

/// <summary>
/// Generates script.rpy content from RenPy novel model.
/// Single Responsibility: Script generation using templates.
/// </summary>
public interface IRenPyScriptGenerator
{
    Task<string> GenerateAsync(RenPyNovel novel, CancellationToken ct = default);
}
