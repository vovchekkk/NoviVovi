namespace NoviVovi.Domain.Images;

public enum ImageType
{
    Background,
    Character,
    Default
}

public static class ImageTypeExtension
{
    public static ImageType ToImageType(this string str)
    {
        return str switch
        {
            "background" => ImageType.Background,
            "character" => ImageType.Character,
            "default" => ImageType.Default,
            _ => throw new Exception("Unknown image type: " + str)
        };
    }

    public static string TypeToString(this ImageType imageType)
    {
        return imageType switch
        {
            ImageType.Background => "background",
            ImageType.Character => "character",
            ImageType.Default => "default",
            _ => throw new Exception("Unknown image type: " + imageType)
        };
    }
}