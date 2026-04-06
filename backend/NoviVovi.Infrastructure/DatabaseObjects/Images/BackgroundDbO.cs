namespace NoviVovi.Infrastructure.DatabaseObjects.Images;

public class BackgroundDbO
{
    public Guid Id { get; set; }
    public Guid Img { get; set; }
    public Guid? TransformId { get; set; }
    
    public ImageDbO? Image { get; set; }
    public TransformDbO? Transform { get; set; }
}