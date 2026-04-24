namespace NoviVovi.Infrastructure.Exporters.RenPy.Services;

public class RenPyHelper
{
    public static string EscapeString(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
}