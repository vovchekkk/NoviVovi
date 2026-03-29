using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.DatabaseObjects.Novels;

public class FullNovelDbO
{
    public NovelDbO Novel { get; set; } = new();
    public List<LabelDbO> Labels { get; set; } = new();
    public List<CharacterDbO> Characters { get; set; } = new();
    public List<ImageDbO> Images { get; set; } = new();
}