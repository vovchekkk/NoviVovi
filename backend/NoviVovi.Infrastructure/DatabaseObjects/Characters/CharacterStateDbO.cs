using NoviVovi.Infrastructure.DatabaseObjects.Images;

namespace NoviVovi.Infrastructure.DatabaseObjects.Characters;

public class CharacterStateDbO
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Guid ImageId { get; set; }
    public string StateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public ImageDbO? Image { get; set; }
}