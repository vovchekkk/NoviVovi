using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services;

/// <summary>
/// Collects all unique image IDs from a novel.
/// Follows Single Responsibility Principle: only handles image collection logic.
/// </summary>
public class NovelImageCollector : INovelImageCollector
{
    public IEnumerable<Guid> CollectImageIds(Novel novel)
    {
        var imageIds = new HashSet<Guid>();

        foreach (var label in novel.Labels)
        {
            foreach (var step in label.Steps)
            {
                switch (step)
                {
                    case ShowBackgroundStep bgStep:
                        imageIds.Add(bgStep.BackgroundObject.Image.Id);
                        break;
                    
                    case ShowCharacterStep charStep:
                        imageIds.Add(charStep.CharacterObject.State.Image.Id);
                        break;
                }
            }
        }

        return imageIds;
    }
}
