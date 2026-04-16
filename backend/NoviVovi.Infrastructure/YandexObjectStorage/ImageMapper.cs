using NoviVovi.Domain.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class ImageMapper
{
    public Image ToImage(ImageDbO imgDbo)
    {
        //var img = new Image(imgDbo.Id, imgDbo.Name, imgDbo.Url, imgDbo.ImgType);
        //return img;
        throw new NotImplementedException();
    }

    //todo! переделать после мерджа
    public ImageDbO ToDbO(Image img)
    {
        var result = new ImageDbO();
        result.Id = img.Id;
        result.Name = img.Name;
        result.ImgType = img.Description;
        return result;
    }
}