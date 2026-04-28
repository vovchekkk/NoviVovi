using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using NoviVovi.Api.Steps.Responses;

namespace NoviVovi.Api.Infrastructure;

public class StepResponseOutputFormatter : TextOutputFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(),
            new TransitionResponseConverter(),
            new StepResponseConverter()
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public StepResponseOutputFormatter()
    {
        SupportedMediaTypes.Add("application/json");
        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanWriteType(Type? type)
    {
        return type == typeof(StepResponse) || 
               (type != null && type.IsSubclassOf(typeof(StepResponse)));
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var response = httpContext.Response;

        // Serialize using our custom options
        var json = JsonSerializer.Serialize(context.Object, typeof(StepResponse), JsonOptions);
        
        await response.WriteAsync(json, selectedEncoding);
    }
}
