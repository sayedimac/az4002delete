using System.Net.Http.Json;

namespace web;

public class ChatService(HttpClient http)
{
    private record GreetResponse(string Message);

    public async Task<string> GreetAsync(string name)
    {
        var response = await http.GetFromJsonAsync<GreetResponse>($"api/greet?name={Uri.EscapeDataString(name)}");
        return response?.Message ?? "Hello!";
    }
}
