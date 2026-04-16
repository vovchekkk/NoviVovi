namespace NoviVovi.Domain.Images;

public enum ImageType
{
    Background,
    Character
}

public static class ImageTypeExtension
{
    public static ImageType ToImageType(this string str)
    {
        return str switch
        {
            "background" => ImageType.Background,
            "character" => ImageType.Character,
            _ => throw new Exception("Unknown image type: " + str)
        };
    }

    public static string TypeToString(this ImageType imageType)
    {
        return imageType switch
        {
            ImageType.Background => "background",
            ImageType.Character => "character",
            _ => throw new Exception("Unknown image type: " + imageType)
        };
    }
}