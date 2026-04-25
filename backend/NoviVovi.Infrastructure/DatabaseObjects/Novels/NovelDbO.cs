using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;

namespace NoviVovi.Infrastructure.DatabaseObjects.Novels;

public class NovelDbO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? StartLabelId { get; set; }
    public Guid? CoverImageId { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }
    
    public List<CharacterDbO> Characters { get; set; } = new();
    public List<LabelDbO> Labels { get; set; } = new();
    public LabelDbO? StartLabel { get; set; }
}