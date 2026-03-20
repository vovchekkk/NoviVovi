using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;

namespace NoviVovi.Domain.Characters;

public class CharacterState : Entity
{
    public string Name { get; private set; }
    public string? Description { get; set; }
    public Image Image { get; private set; }

    private CharacterState(Guid id, string name, Image image, string? description) : base(id)
    {
        Name = name;
        Description = description;
        Image = image;
    }

    public static CharacterState Create(
        Image image,
        string? name,
        string? description = null)
    {
        if (image is null)
            throw new DomainException($"NextAction cannot be null");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");

        return new CharacterState(Guid.NewGuid(), name, image, description);
    }
}