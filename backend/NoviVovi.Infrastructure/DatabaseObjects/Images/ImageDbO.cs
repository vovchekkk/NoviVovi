namespace NoviVovi.Infrastructure.DatabaseObjects.Images;

public class ImageDbO
{
    public Guid Id { get; set; }
    public Guid? NovelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string ImgType { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Width { get; set; }
    public int Size { get; set; }
}
