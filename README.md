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

In Development:

- OpenAPI document: [`/openapi/v1.json`](http://localhost:5270/openapi/v1.json)
- Swagger UI: [`/swagger`](http://localhost:5270/swagger) — use the **Authorize** button with the JWT from `/auth/login` to call protected endpoints.

Both are gated to `IsDevelopment()` and won't be exposed in Production.

## Authentication

The API uses JWT bearer authentication. `POST /auth/login` issues a token, and `[Authorize]` endpoints (e.g. `/weatherforecast`) require it.

```bash
# 1. Get a token (demo credentials).
TOKEN=$(curl -s -X POST http://localhost:5270/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"password"}' | jq -r .accessToken)

# 2. Call a protected endpoint.
curl http://localhost:5270/weatherforecast -H "Authorization: Bearer $TOKEN"
```

Without the bearer header, protected endpoints return `401 Unauthorized`.

JWT settings live under the `Jwt` configuration section:

| Key | Description |
| --- | --- |
| `Jwt:Issuer` | Token issuer (`iss`). |
| `Jwt:Audience` | Token audience (`aud`). |
| `Jwt:Key` | Symmetric signing key. **Required.** Keep out of source control — use user-secrets, `Jwt__Key` env var, or a key vault. |
| `Jwt:ExpiresMinutes` | Token lifetime in minutes (default `60`). |

`appsettings.Development.json` ships a placeholder dev key so the app boots locally; the demo credentials (`demo` / `password`) in `AuthController` are placeholders — replace them with a real user store before shipping.

Or use Swagger UI at [`/swagger`](http://localhost:5270/swagger): expand `POST /auth/login`, send the demo credentials, copy the `accessToken` from the response, click **Authorize**, paste the token, and re-run protected endpoints from the browser.

Or open [`src/Vibe.AspNetCore/Vibe.AspNetCore.http`](src/Vibe.AspNetCore/Vibe.AspNetCore.http) in an editor with REST-client support: run the login request, paste the returned `accessToken` into the `@token` variable, and send the weather request.

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
    └── Vibe.AspNetCore/      # ASP.NET Core Web API (controllers + OpenAPI + JWT)
        ├── Authentication/   # JwtOptions
        ├── Controllers/      # AuthController, WeatherForecastController
        ├── OpenApi/          # Bearer security scheme + per-operation requirement transformers
        ├── Program.cs
        └── Vibe.AspNetCore.csproj
```

`Program.cs` uses minimal hosting with attribute-routed controllers, `Microsoft.AspNetCore.OpenApi` (with `Swashbuckle.AspNetCore.SwaggerUI` on top of it for the Swagger UI page), and JWT bearer authentication. The `WeatherForecast` controller and model are template scaffolding — replace them with real endpoints.
