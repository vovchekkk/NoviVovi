using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Images;

public class Image : Entity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Url { get; set; }
    public string Format { get; set; }
    public string Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Image(
        Guid id,
        string name,
        string url,
        string format,
        string type,
        int width,
        int height,
        string? description = null
    ) : base(id)
    {
        Id = id;
        Name = name;
        Url = url;
        Format = format;
        Type = type;
        Width = width;
        Height = height;
        Description = description;
    }

    public static Image Create(
        string name,
        string url,
        string format,
        string type,
        int width,
        int height,
        string? description = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");

        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException($"Url cannot be null");

        if (string.IsNullOrWhiteSpace(format))
            throw new DomainException($"Format cannot be empty");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException($"Type cannot be empty");

        if (width <= 0 || height <= 0)
            throw new DomainException($"Width and height cannot be zero or negative");

        return new Image(
            Guid.NewGuid(),
            name,
            url,
            format,
            type,
            width,
            height,
            description
        );
    }
}