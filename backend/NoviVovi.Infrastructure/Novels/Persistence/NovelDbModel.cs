namespace NoviVovi.Infrastructure.Novels.Persistence;

public class NovelDbModel
{
    public Guid Id;
    public string Title { get; set; }
    public Guid StartLabelId { get; set; }
    public List<Guid> LabelIds { get; set; }
}