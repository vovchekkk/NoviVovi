using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class ImageMapper(
    TransformMapper mapper
)
{
    public Image ToDomain(ImageDbO imgDbo)
    {
        var img = new Image(
            imgDbo.Id,
            imgDbo.Name,
            imgDbo.NovelId,
            imgDbo.Url,
            imgDbo.Format,
            imgDbo.ImgType.ToImageType(),
            new Size(imgDbo.Width, imgDbo.Height),
            null,
            ImageStatus.Active);
        return img;
    }

    public ImageDbO ToDbO(Image img)
    {
        var result = new ImageDbO
        {
            Id = img.Id,
            Name = img.Name,
            Url = img.StoragePath,
            ImgType = img.Type.TypeToString(),
            Width = img.Size.Width,
            Height = img.Size.Height,
            Format = img.Format,
            NovelId = img.NovelId
        };
        return result;
    }

    public BackgroundObject ToDomain(BackgroundDbO background)
    {
        if (background.Image != null)
        {
            var result = new BackgroundObject(
                background.Id,
                ToDomain(background.Image),
                mapper.ToDomain(background.Transform)
            );
            return result;
        }
        
        throw new ArgumentException("Bg object should have image");
    }

    public BackgroundDbO ToDbO(BackgroundObject bg, Guid novelId)
    {
        var result = new BackgroundDbO
        {
            Id = bg.Id,
            Img = bg.Image.Id,
            Transform = mapper.ToDbO(bg.Transform),
            TransformId = bg.Transform.Id,
            Image = ToDbO(bg.Image)
        };
        return result;
    }
}