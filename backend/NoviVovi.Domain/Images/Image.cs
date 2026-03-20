namespace NoviVovi.Domain.Images;

public class Image
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string? Description { get; set; }
    
    private Image(Guid id, string name, string url, string? description = null)
    {
        Id = id;
        Name = name;
        Url = url;
        Description = description;
    }
}