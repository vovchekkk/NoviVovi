namespace NoviVovi.Application.Novels.Get;

public class GetNovelQuery(Guid id)
{
    public Guid Id { get; set; } = id;
}