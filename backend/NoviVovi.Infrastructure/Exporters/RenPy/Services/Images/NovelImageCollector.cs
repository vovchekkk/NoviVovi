using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;

/// <summary>
/// Collects all unique images from a novel with RenPy naming information.
/// Follows Single Responsibility Principle: only handles image collection logic.
/// </summary>
public class NovelImageCollector : INovelImageCollector
{
    public IEnumerable<ImageExportInfo> CollectImages(Novel novel)
    {
        var images = new List<ImageExportInfo>();

        foreach (var label in novel.Labels)
        {
            foreach (var step in label.Steps)
            {
                switch (step)
                {
                    case ShowBackgroundStep bgStep:
                        images.Add(new ImageExportInfo(
                            bgStep.BackgroundObject.Image.Id,
                            bgStep.BackgroundObject.Image.StoragePath,
                            $"bg_{bgStep.BackgroundObject.Image.Id:N}"
                        ));
                        break;
                    
                    case ShowCharacterStep charStep:
                        var charId = charStep.CharacterObject.Character.Id;
                        var stateId = charStep.CharacterObject.State.Id;
                        images.Add(new ImageExportInfo(
                            charStep.CharacterObject.State.Image.Id,
                            charStep.CharacterObject.State.Image.StoragePath,
                            $"char_{charId:N}_state_{stateId:N}"
                        ));
                        break;
                }
            }
        }

        return images.DistinctBy(x => x.ImageId);
    }
}
