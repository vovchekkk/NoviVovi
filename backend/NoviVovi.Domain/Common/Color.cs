using System.Text.RegularExpressions;

namespace NoviVovi.Domain.Common;

/// <summary>
/// Value object representing a color in HEX format (#RRGGBB).
/// </summary>
public partial class Color : ValueObject
{
    public string Value { get; }

    private Color(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a Color from a HEX string. Supports #RRGGBB and #RGB formats.
    /// </summary>
    /// <param name="hex">HEX color string (e.g., "#FF5733" or "#F73")</param>
    /// <returns>Color instance</returns>
    /// <exception cref="DomainException">Thrown when the HEX format is invalid</exception>
    public static Color FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new DomainException("Color cannot be empty");

        // Normalize: add # if missing
        var normalized = hex.StartsWith("#") ? hex : $"#{hex}";

        // Validate: #RRGGBB or #RGB
        if (!IsValidHex(normalized))
            throw new DomainException($"Invalid color format: '{hex}'. Expected #RRGGBB or #RGB");

        // Expand short form: #RGB -> #RRGGBB
        if (normalized.Length == 4)
        {
            normalized = $"#{normalized[1]}{normalized[1]}{normalized[2]}{normalized[2]}{normalized[3]}{normalized[3]}";
        }

        return new Color(normalized.ToUpperInvariant());
    }

    /// <summary>
    /// Creates a default white color (#FFFFFF).
    /// </summary>
    public static Color Default() => FromHex("#FFFFFF");

    private static bool IsValidHex(string hex)
    {
        if (hex.Length != 7 && hex.Length != 4)
            return false;

        if (hex[0] != '#')
            return false;

        return HexPattern().IsMatch(hex);
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{3}$|^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexPattern();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
