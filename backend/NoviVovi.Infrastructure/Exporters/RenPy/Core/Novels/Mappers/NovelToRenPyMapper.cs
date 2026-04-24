using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Services;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Mappers;

/// <summary>
/// Maps Domain Novel to RenPy project structure.
/// Orchestrates other mappers to build complete RenPy project.
/// </summary>
public class NovelToRenPyMapper(
    RenPyIdentifierGenerator idGenerator,
    CharacterToRenPyMapper characterMapper,
    LabelToRenPyMapper labelMapper
)
{
    public RenPyNovel Map(Novel novel)
    {
        return new RenPyNovel
        {
            Title = novel.Title,
            Characters = novel.Characters.Select(characterMapper.Map).ToList(),
            Labels = novel.Labels.Select(labelMapper.Map).ToList(),
            StartLabelId = idGenerator.GenerateForLabel(novel.StartLabel.Id)
        };
    }
}