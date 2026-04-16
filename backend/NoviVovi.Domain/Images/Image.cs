using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Images;

public class Image : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string StoragePath { get; private set; } 
    public string Format { get; private set; }
    public ImageType Type { get; private set; }
    public Size Size { get; private set; }
    public ImageStatus Status { get; private set; }

    private Image(
        Guid id,
        string name,
        string storagePath,
        string format,
        ImageType type,
        Size size,
        string? description,
        ImageStatus status
    ) : base(id)
    {
        Name = name;
        StoragePath = storagePath;
        Format = format;
        Type = type;
        Size = size;
        Description = description;
        Status = status;
    }
    
    public static Image CreatePending(
        string name,
        string storagePath,
        string format,
        ImageType type,
        Size size,
        string? description = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");
        
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new DomainException($"StoragePath cannot be empty");
        
        if (string.IsNullOrWhiteSpace(format))
            throw new DomainException($"Format cannot be empty");
        
        if (size is null)
            throw new DomainException($"Size cannot be null");
        
        if (size.Width <= 0 || size.Height <= 0)
            throw new DomainException($"Invalid dimensions");

        return new Image(
            Guid.NewGuid(),
            name,
            storagePath,
            format,
            type,
            size,
            description,
            ImageStatus.Pending
        );
    }
    
    public void ConfirmUpload()
    {
        if (Status == ImageStatus.Active)
            return;
        Status = ImageStatus.Active;
    }

    public void UpdateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        
        Name = name;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }
    
    public void UpdateFormat(string? format)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new DomainException("Format cannot be empty");
        
        Format = format;
    }

    public void UpdateType(ImageType? type)
    {
        if (!type.HasValue)
            throw new DomainException("ImageType cannot be empty");
        
        Type = type.Value;
    }
    
    public void UpdateSize(Size? size)
    {
        if (size is null)
            throw new DomainException($"Size cannot be null");
        
        if (size.Width <= 0 || size.Height <= 0)
            throw new DomainException($"Invalid dimensions");
        
        Size = size;
    }
    
    public void UpdateSize(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new DomainException($"Invalid dimensions");
        
        Size = new Size(width, height);
    }
}