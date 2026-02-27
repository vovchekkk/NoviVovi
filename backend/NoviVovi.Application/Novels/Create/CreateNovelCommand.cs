namespace NoviVovi.Application.Novels.Create;

public class CreateNovelCommand(string title)
{
    public string Title { get; } = title;
}