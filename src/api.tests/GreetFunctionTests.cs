using CopilotApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace api.tests;

/// <summary>
/// Unit tests for the GreetFunction HTTP trigger.
/// </summary>
public class GreetFunctionTests
{
    private static GreetFunction CreateFunction() =>
        new(NullLogger<GreetFunction>.Instance);

    // ── helper ───────────────────────────────────────────────────────────────

    private static HttpRequest BuildGetRequest(string? name = null)
    {
        var context = new DefaultHttpContext();
        if (name is not null)
            context.Request.QueryString = new QueryString($"?name={Uri.EscapeDataString(name)}");
        return context.Request;
    }

    // ── tests ────────────────────────────────────────────────────────────────

    [Fact]
    public void Greet_WithName_ReturnsPersonalizedGreeting()
    {
        var function = CreateFunction();
        var request = BuildGetRequest("Alice");

        var result = function.Run(request) as OkObjectResult;

        Assert.NotNull(result);
        var json = result.Value?.ToString();
        Assert.Contains("Alice", json);
    }

    [Fact]
    public void Greet_WithoutName_ReturnsHelloWorld()
    {
        var function = CreateFunction();
        var request = BuildGetRequest();

        var result = function.Run(request) as OkObjectResult;

        Assert.NotNull(result);
        var json = result.Value?.ToString();
        Assert.Contains("World", json);
    }

    [Fact]
    public void Greet_ReturnsOkStatusCode()
    {
        var function = CreateFunction();
        var request = BuildGetRequest("Bob");

        var result = function.Run(request) as OkObjectResult;

        Assert.Equal(200, result?.StatusCode);
    }

    [Theory]
    [InlineData("Alice", "Hello, Alice!")]
    [InlineData("Bob", "Hello, Bob!")]
    [InlineData("", "Hello, World!")]
    public void Greet_VariousNames_ReturnsExpectedMessage(string name, string expectedMessage)
    {
        var function = CreateFunction();
        var request = BuildGetRequest(string.IsNullOrEmpty(name) ? null : name);

        var result = function.Run(request) as OkObjectResult;

        Assert.NotNull(result);
        var json = result.Value?.ToString();
        Assert.Contains(expectedMessage, json);
    }
}
