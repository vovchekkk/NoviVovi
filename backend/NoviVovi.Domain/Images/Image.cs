using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Images;

public class Image : Entity
{
    public string Name { get; private set; }
    public Guid? NovelId { get; private set; }
    public string StoragePath { get; private set; } 
    public string Format { get; private set; }
    public ImageType Type { get; private set; }
    public Size Size { get; private set; }
    public ImageStatus Status { get; private set; }

    public Image(
        Guid id,
        string name,
        Guid? novelId,
        string storagePath,
        string format,
        ImageType type,
        Size size,
        ImageStatus status
    ) : base(id)
    {
        Name = name;
        NovelId = novelId;
        StoragePath = storagePath;
        Format = format;
        Type = type;
        Size = size;
        Status = status;
    }
    
    public static Image CreatePending(
        string? name,
        Guid novelId,
        string? storagePath,
        string? format,
        ImageType type,
        Size? size,
        Guid? imageGuid = null
    )
    {
        imageGuid ??= Guid.NewGuid();
        
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");
        
        if (novelId == Guid.Empty)
            throw new DomainException($"NovelId cannot be empty");
        
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new DomainException($"StoragePath cannot be empty");
        
        if (string.IsNullOrWhiteSpace(format))
            throw new DomainException($"Format cannot be empty");
        
        // Validate format - only allow common image formats
        var allowedFormats = new[] { "png", "jpg", "jpeg", "webp", "gif" };
        if (!allowedFormats.Contains(format.ToLowerInvariant()))
            throw new DomainException($"Invalid image format '{format}'. Allowed formats: {string.Join(", ", allowedFormats)}");
        
        if (size is null)
            throw new DomainException($"Size cannot be null");
        
        if (size.Width <= 0 || size.Height <= 0)
            throw new DomainException($"Invalid dimensions");

        return new Image(
            imageGuid.Value,
            name,
            novelId,
            storagePath,
            format,
            type,
            size,
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

    public void UpdateType(ImageType? type)
    {
        if (!type.HasValue)
            throw new DomainException("ImageType cannot be empty");
        
        Type = type.Value;
    }
}