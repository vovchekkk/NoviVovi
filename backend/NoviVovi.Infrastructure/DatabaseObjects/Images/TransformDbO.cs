namespace NoviVovi.Infrastructure.DatabaseObjects.Images;

public class TransformDbO
{
    public Guid Id { get; set; }
    public decimal? Scale { get; set; } = 1;
    public decimal? Rotation { get; set; } = 0;
    public int? ZIndex { get; set; } = 0;
    public int Width { get; set; }
    public int Height { get; set; }
    public decimal? XPos { get; set; } = 0;
    public decimal? YPos { get; set; } = 0;
}