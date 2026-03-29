namespace NoviVovi.Infrastructure.DatabaseObjects.Images;

public class TransformDbO
{
    public Guid Id { get; set; }
    public decimal? Scale { get; set; }
    public decimal? Rotation { get; set; }
    public int? ZIndex { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public decimal? XPos { get; set; }
    public decimal? YPos { get; set; }
}