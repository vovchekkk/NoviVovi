using System.Text.Json;
using NoviVovi.Api.Menu.Responses;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Tests.Serialization;

public class StepResponseSerializationTest
{
    [Fact]
    public void TestShowMenuStepResponseSerialization()
    {
        // Arrange
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
                    ),
                    new(
                        Text: "Choice 2", 
                        Transition: new ChoiceTransitionResponse { TargetLabelId = Guid.NewGuid() }
                    )
                }
            ),
            Transition = new NextStepTransitionResponse()
        };

        // Act - serialize as base type
        var json = JsonSerializer.Serialize<StepResponse>(response);
        Console.WriteLine("Serialized JSON:");
        Console.WriteLine(json);

        // Check if 'type' field is present
        Assert.Contains("\"type\":", json);
        Assert.Contains("\"show_menu\"", json);
        
        // Try to deserialize
        var deserialized = JsonSerializer.Deserialize<StepResponse>(json);
        
        // Assert
        Assert.NotNull(deserialized);
        Assert.IsType<ShowMenuStepResponse>(deserialized);
    }
}
