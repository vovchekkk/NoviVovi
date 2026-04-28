using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Characters;

public class CharacterState : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Image Image { get; private set; }
    public Transform LocalTransform { get; private set; }

    public CharacterState(Guid id, string name, Image image, Transform localTransform, string? description) : base(id)
    {
        Name = name;
        Description = description;
        Image = image;
        LocalTransform = localTransform;
    }

    public static CharacterState Create(
        string? name,
        Image? image,
        Transform? localTransform,
        string? description = null)
    {
        if (image is null)
            throw new DomainException($"Image cannot be null");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");

        if (localTransform is null)
            throw new DomainException($"LocalTransform cannot be null");

        return new CharacterState(Guid.NewGuid(), name, image, localTransform, description);
    }

    public void UpdateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");

        Name = name;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateImage(Image? image)
    {
        if (image is not null)
            Image = image;
    }

    public void PatchTransform(TransformPatch? transformPatch)
    {
        if (transformPatch is not null)
            LocalTransform = LocalTransform.ApplyPatch(transformPatch);
    }
}