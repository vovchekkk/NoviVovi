using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.Exporters.RenPy.Generators;
using NoviVovi.Infrastructure.Exporters.RenPy.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Mappers;

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