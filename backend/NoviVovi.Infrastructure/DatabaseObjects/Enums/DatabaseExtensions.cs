using NoviVovi.Infrastructure.DatabaseObjects.Enums;

namespace NoviVovi.Infrastructure;

public static class DbExtensions
{
    public static StepType ToStepType(this string stepType)
    {
        return stepType switch
        {
            "replica" => StepType.ShowReplica,
            "menu" => StepType.ShowMenu,
            "background" => StepType.ShowBackground,
            "jump" => StepType.Jump,
            "show_character" => StepType.ShowCharacter,
            "hide_character" => StepType.HideCharacter,
            _ => throw new ArgumentException("такого в энуме быть не должно"),
        };
    }

    public static string ToStepTypeString(this StepType stepType)
    {
        return stepType switch
        {
            StepType.HideCharacter => "hide_character",
            StepType.Jump => "jump",
            StepType.ShowBackground => "show_background",
            StepType.ShowMenu => "show_menu",
            StepType.ShowReplica => "show_replica",
            StepType.ShowCharacter => "show_character",
        };
    }

    public static ImageType ToImageType(this string imgType)
    {
        return imgType switch
        {
            "character" => ImageType.Character,
            "background" => ImageType.Background,
            "cover" => ImageType.Cover,
            "icon" => ImageType.Icon,
            _ => ImageType.Character
        };
    }

    public static string ToImageTypeString(this ImageType imageType)
    {
        return imageType switch
        {
            ImageType.Character => "character",
            ImageType.Background => "background",
            ImageType.Cover => "cover",
            ImageType.Icon => "icon",
            _ => "character"
        };
    }
}
