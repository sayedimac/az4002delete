using System.Net;
using System.Net.Http.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using web;
using web.Pages;

namespace web.tests;

/// <summary>
/// bUnit UI tests for the Home (chat) page component.
/// These run entirely in-process — no browser required —
/// making them suitable for Copilot / CI environments.
/// </summary>
public class HomePageTests : TestContext
{
    private Mock<ChatService> CreateChatServiceMock(string replyMessage)
    {
        var httpClient = new HttpClient(new StaticReplyHandler(replyMessage))
        {
            BaseAddress = new Uri("http://localhost/")
        };
        // Return a real ChatService backed by a stub handler
        var svc = new ChatService(httpClient);
        var mock = new Mock<ChatService>(httpClient) { CallBase = true };
        return mock;
    }

    // ── helper: stub HttpMessageHandler ─────────────────────────────────────

    private sealed class StaticReplyHandler(string message) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var payload = new { message };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(payload)
            };
            return Task.FromResult(response);
        }
    }

    // ── helper: register a ChatService that always returns a fixed reply ─────

    private ChatService RegisterFakeChatService(string reply)
    {
        var httpClient = new HttpClient(new StaticReplyHandler(reply))
        {
            BaseAddress = new Uri("http://localhost/")
        };
        var svc = new ChatService(httpClient);
        Services.AddSingleton(svc);
        return svc;
    }

    // ── tests ────────────────────────────────────────────────────────────────

    [Fact]
    public void HomePage_RendersWelcomePrompt_WhenNoMessages()
    {
        RegisterFakeChatService("Hello, Test!");

        var cut = RenderComponent<Home>();

        Assert.Contains("Welcome", cut.Markup);
    }

    [Fact]
    public void HomePage_HasChatInput()
    {
        RegisterFakeChatService("Hello!");

        var cut = RenderComponent<Home>();

        var input = cut.Find("input#chat-input");
        Assert.NotNull(input);
    }

    [Fact]
    public void HomePage_SendButton_IsDisabledWhenInputIsEmpty()
    {
        RegisterFakeChatService("Hello!");

        var cut = RenderComponent<Home>();

        var btn = cut.Find("button");
        Assert.True(btn.HasAttribute("disabled"),
            "Send button should be disabled when the input is empty.");
    }

    [Fact]
    public async Task HomePage_AfterSendingName_ShowsUserBubble()
    {
        RegisterFakeChatService("Hello, Alice!");

        var cut = RenderComponent<Home>();

        var input = cut.Find("input#chat-input");
        await input.InputAsync(new() { Value = "Alice" });

        var btn = cut.Find("button");
        await btn.ClickAsync(new());

        Assert.Contains("Alice", cut.Markup);
    }

    [Fact]
    public async Task HomePage_AfterSendingName_ShowsBotReply()
    {
        RegisterFakeChatService("Hello, Bob!");

        var cut = RenderComponent<Home>();

        var input = cut.Find("input#chat-input");
        await input.InputAsync(new() { Value = "Bob" });

        await cut.Find("button").ClickAsync(new());

        Assert.Contains("Hello, Bob!", cut.Markup);
    }

    [Fact]
    public async Task HomePage_InputClearedAfterSend()
    {
        RegisterFakeChatService("Hi!");

        var cut = RenderComponent<Home>();

        var input = cut.Find("input#chat-input");
        await input.InputAsync(new() { Value = "Charlie" });
        await cut.Find("button").ClickAsync(new());

        var inputAfter = cut.Find("input#chat-input");
        Assert.Equal(string.Empty, inputAfter.GetAttribute("value") ?? string.Empty);
    }
}
