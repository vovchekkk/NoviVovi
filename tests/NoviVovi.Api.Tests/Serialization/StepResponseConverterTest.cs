using System.Text.Json;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Api.Menu.Responses;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Tests.Serialization;

public class StepResponseConverterTest
{
    [Fact]
    public void StepResponseConverter_Serialization_AddsTypeDiscriminator()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        options.Converters.Add(new StepResponseConverter());
        options.Converters.Add(new TransitionResponseConverter());
        
        var targetLabelId = Guid.NewGuid();
        var response = new ShowMenuStepResponse
        {
            Id = Guid.NewGuid(),
            Menu = new MenuResponse(
                Choices: new List<ChoiceResponse>
                {
                    new(
                        Text: "Choice 1", 
                        Transition: new ChoiceTransitionResponse { TargetLabelId = targetLabelId }
                    )
                }
            ),
            Transition = new NextStepTransitionResponse()
        };

        // Act - serialize as base type
        var json = JsonSerializer.Serialize<StepResponse>(response, options);
        
        Console.WriteLine("Serialized JSON:");
        Console.WriteLine(json);

        // Assert - check type discriminator is present
        Assert.Contains("\"type\":\"show_menu\"", json);
        
        // Try to deserialize
        var deserialized = JsonSerializer.Deserialize<StepResponse>(json, options);
        
        Assert.NotNull(deserialized);
        Assert.IsType<ShowMenuStepResponse>(deserialized);
        
        var menuStep = (ShowMenuStepResponse)deserialized;
        Assert.NotNull(menuStep.Menu);
        Assert.Single(menuStep.Menu.Choices);
    }
}
