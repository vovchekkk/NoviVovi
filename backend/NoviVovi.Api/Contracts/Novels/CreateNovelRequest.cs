namespace NoviVovi.Api.Contracts.Novels;

public class CreateNovelRequest
{
    public string Title { get; set; } = null!; 
    public List<string>? Slides { get; set; }
}