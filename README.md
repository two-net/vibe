# vibe

An ASP.NET Core Web API on .NET 10.

## Requirements

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) or newer

## Getting started

```bash
dotnet restore
dotnet run --project src/Vibe.AspNetCore
```

The app listens on:

- HTTP — `http://localhost:5270`
- HTTPS — `https://localhost:7198` (use `--launch-profile https`)

In Development, the OpenAPI document is served at `/openapi/v1.json`.

Try the sample endpoint:

```bash
curl http://localhost:5270/weatherforecast
```

Or open [`src/Vibe.AspNetCore/Vibe.AspNetCore.http`](src/Vibe.AspNetCore/Vibe.AspNetCore.http) in an editor with REST-client support and send the request from there.

## Common commands

| Task | Command |
| --- | --- |
| Build | `dotnet build` |
| Run with hot reload | `dotnet watch --project src/Vibe.AspNetCore` |
| Format | `dotnet format` |
| Publish (Release) | `dotnet publish -c Release` |

## Project layout

```
vibe.sln
└── src/
    └── Vibe.AspNetCore/      # ASP.NET Core Web API (controllers + OpenAPI)
        ├── Controllers/
        ├── Program.cs
        └── Vibe.AspNetCore.csproj
```

`Program.cs` uses minimal hosting with attribute-routed controllers and `Microsoft.AspNetCore.OpenApi`. The `WeatherForecast` controller and model are template scaffolding — replace them with real endpoints.
