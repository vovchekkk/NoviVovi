using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Domain.Scene;

public class BackgroundObject : SceneObject
{
    public Image Image { get; private set; }

    public BackgroundObject(Guid id, Image image, Transform transform)
        : base(id, transform)
    {
        Image = image;
    }

    public static BackgroundObject Create(Image? image, Transform? transform)
    {
        if (image is null)
            throw new DomainException($"Image cannot be null");

        if (transform is null)
            throw new DomainException($"Transform cannot be null");

        return new BackgroundObject(Guid.NewGuid(), image, transform);
    }

    public void UpdateImage(Image? image)
    {
        Image = image
                ?? throw new DomainException($"Image cannot be null");
    }
}