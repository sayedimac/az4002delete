# CopilotTest — Blazor WASM + .NET 10 Azure Functions

A minimal chatbot-style static web app using:

- **Blazor WebAssembly** (.NET 10) — `src/web`
- **Azure Functions** (.NET 10, isolated mode) — `src/api`
- **Bootstrap 5** for styling
- **bUnit + xUnit** for UI component tests — `src/web.tests`
- **xUnit** for Function tests — `src/api.tests`

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Azure Functions Core Tools v4+](https://learn.microsoft.com/azure/azure-functions/functions-run-local)

### Build

```bash
dotnet build
```

### Run the Blazor app locally

```bash
dotnet run --project src/web
```

### Run the Function API locally

```bash
cd src/api
func start
```

### Run tests

```bash
dotnet test
```

## Project structure

```
CopilotTest.sln
src/
  web/             ← Blazor WebAssembly (.NET 10)
    Pages/
      Home.razor   ← Chat UI
    ChatService.cs ← API client
  api/             ← Azure Functions (.NET 10 isolated)
    GreetFunction.cs
  web.tests/       ← bUnit UI component tests
  api.tests/       ← xUnit Function unit tests
```

## Deploying to Azure Static Web Apps

Set the secret `AZURE_STATIC_WEB_APPS_API_TOKEN` in your repository and push to `main`.
