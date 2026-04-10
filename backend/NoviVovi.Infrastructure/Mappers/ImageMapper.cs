using NoviVovi.Domain.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class ImageMapper
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
}