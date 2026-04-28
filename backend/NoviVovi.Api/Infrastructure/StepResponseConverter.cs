using System.Text.Json;
using System.Text.Json.Serialization;
using NoviVovi.Api.Steps.Responses;

namespace NoviVovi.Api.Infrastructure;

public class StepResponseConverter : JsonConverter<StepResponse>
{
    public override StepResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Get the type discriminator
        if (!root.TryGetProperty("type", out var typeProperty))
        {
            throw new JsonException("Missing 'type' discriminator in StepResponse JSON");
        }

        var type = typeProperty.GetString();
        var json = root.GetRawText();

        // Create new options without this converter to avoid recursion
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Clear();
        
        // Add back all converters except this one
        foreach (var converter in options.Converters)
        {
            if (converter is not StepResponseConverter)
            {
                newOptions.Converters.Add(converter);
            }
        }

        // Deserialize based on type
        return type switch
        {
            "show_replica" => JsonSerializer.Deserialize<ShowReplicaStepResponse>(json, newOptions),
            "show_menu" => JsonSerializer.Deserialize<ShowMenuStepResponse>(json, newOptions),
            "show_character" => JsonSerializer.Deserialize<ShowCharacterStepResponse>(json, newOptions),
            "hide_character" => JsonSerializer.Deserialize<HideCharacterStepResponse>(json, newOptions),
            "show_background" => JsonSerializer.Deserialize<ShowBackgroundStepResponse>(json, newOptions),
            "jump" => JsonSerializer.Deserialize<JumpStepResponse>(json, newOptions),
            _ => throw new JsonException($"Unknown step type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, StepResponse value, JsonSerializerOptions options)
    {
        // Create new options without this converter to avoid recursion
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Clear();
        
        // Add back all converters except this one
        foreach (var converter in options.Converters)
        {
            if (converter is not StepResponseConverter)
            {
                newOptions.Converters.Add(converter);
            }
        }

        // Serialize the object first
        var jsonElement = JsonSerializer.SerializeToElement(value, value.GetType(), newOptions);
        
        // Write with type discriminator
        writer.WriteStartObject();
        
        // Add type discriminator first
        writer.WriteString("type", GetTypeDiscriminator(value));
        
        // Write all other properties
        foreach (var property in jsonElement.EnumerateObject())
        {
            property.WriteTo(writer);
        }
        
        writer.WriteEndObject();
    }

    private static string GetTypeDiscriminator(StepResponse value) => value switch
    {
        ShowReplicaStepResponse => "show_replica",
        ShowMenuStepResponse => "show_menu",
        ShowCharacterStepResponse => "show_character",
        HideCharacterStepResponse => "hide_character",
        ShowBackgroundStepResponse => "show_background",
        JumpStepResponse => "jump",
        _ => throw new NotSupportedException($"Unknown step type: {value.GetType().Name}")
    };
}
