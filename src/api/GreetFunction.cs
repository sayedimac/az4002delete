using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CopilotApi;

public class GreetFunction(ILogger<GreetFunction> logger)
{
    [Function("greet")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "greet")] HttpRequest req)
    {
        logger.LogInformation("Greet function triggered.");

        string? name = req.Query["name"];

        if (string.IsNullOrWhiteSpace(name)
            && req.HasJsonContentType()
            && req.ContentLength > 0)
        {
            using var reader = new StreamReader(req.Body);
            var body = reader.ReadToEnd();
            var json = System.Text.Json.JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("name", out var nameProp))
            {
                name = nameProp.GetString();
            }
        }

        var greeting = string.IsNullOrWhiteSpace(name)
            ? "Hello, World!"
            : $"Hello, {name}!";

        return new OkObjectResult(new { message = greeting });
    }
}
