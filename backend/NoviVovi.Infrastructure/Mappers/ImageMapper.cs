using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class ImageMapper(TransformMapper mapper)
{
    public Image ToDomain(ImageDbO imgDbo)
    {
        var img = new Image(
            imgDbo.Id,
            imgDbo.Name,
            imgDbo.Url,
            imgDbo.Format,
            imgDbo.ImgType,
            imgDbo.Width,
            imgDbo.Height); 
        return img;
    }

    public ImageDbO ToDbO(Image img, Guid novelId)
    {
        var result = new ImageDbO
        {
            Id = img.Id,
            Name = img.Name,
            Url = img.Url,
            ImgType = img.Type,
            Width = img.Width,
            Height = img.Height,
            Format = img.Format,
            NovelId = novelId
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
            TransformId = Guid.Empty,  //TODO: саня, выпили Id из трансформов 
            Image = ToDbO(bg.Image, novelId)
        };
        return result;
    }
}